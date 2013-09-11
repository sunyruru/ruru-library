namespace Ruru.Net
{
    using System;
    using System.Net;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;
    using System.Security.Principal;
    using System.Threading;

    /// <summary>
    /// 네트워크 경로에 대한 연결
    /// </summary>
    public class NetworkConnection : IDisposable
    {
        string _networkName;

        [System.Runtime.InteropServices.DllImport("mpr.dll")]
        private static extern int WNetAddConnection2(NetResource netResource, string password, string username, int flags);

        [System.Runtime.InteropServices.DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2(string name, int flags, bool force);

        /// <summary>
        /// 네트워크 경로에 대한 연결
        /// </summary>
        /// <param name="networkName"></param>
        /// <param name="credentials"></param>
        /// <exception cref="System.ComponentModel.Win32Exception">이 Credential 을 가지고 있는 프로그램이 아직 열려 있으면 아래 오류가 나타나더라...</exception>
        public NetworkConnection(string networkName, NetworkCredential credentials)
        {
            _networkName = networkName;

            var netResource = new NetResource()
            {
                Scope = ResourceScope.GlobalNetwork,
                ResourceType = ResourceType.Disk,
                DisplayType = ResourceDisplaytype.Share,
                RemoteName = networkName
            };

            int result = WNetAddConnection2(
                netResource,
                credentials.Password,
                (credentials.Domain.Length > 0) ? credentials.Domain + "\\" + credentials.UserName : credentials.UserName,
                0);

            if (result != 0)
            {
                throw new System.ComponentModel.Win32Exception(result, "Error connecting to remote share");
            }
        }

        /// <summary>
        /// 소멸자
        /// </summary>
        ~NetworkConnection()
        {
            Dispose(false);
        }

        #region Network Connection Static method
        /// <summary>
        /// 지속적인 네트워크 연결 등록.
        /// 타 프로세스 실행 시 경로 인증을 받기 위해 PC 자체에 인증을 기록한다 (NET USE 명령어)
        /// </summary>
        /// <param name="Path">경로</param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string OpenNetworkConnection(string Path, string username, string password)
        {
            string sResult = string.Empty;
            string sDir = string.Empty;
            Ruru.Common.OS.ConsoleUtil oConsole = null;

            try
            {
                oConsole = new Common.OS.ConsoleUtil();
                sDir = (Path.IndexOf('$') == -1) ? Path : System.IO.Path.GetDirectoryName(Path).Trim();

                sResult = oConsole.Run("cmd.exe", string.Format("/C \"NET USE \"{0}\" /user:{1} {2}\"", sDir, username, password));
            }
            catch (Exception)
            {
                throw;
            }

            return sResult;
        }

        /// <summary>
        /// 지속적인 네트워크 연결 해제.
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public static string CloseNetworkConnection(string Path)
        {
            string sResult = string.Empty;
            string sDir = string.Empty;
            Ruru.Common.OS.ConsoleUtil oConsole = null;

            try
            {
                oConsole = new Common.OS.ConsoleUtil();
                sDir = (Path.IndexOf('$') == -1) ? Path : System.IO.Path.GetDirectoryName(Path).Trim();

                sResult = oConsole.Run("cmd.exe", string.Format("/C \"NET USE \"{0}\" /delete\"", sDir));
            }
            catch (Exception)
            {
                throw;
            }

            return sResult;
        }

        /// <summary>
        /// http://pinvoke.net/default.aspx/advapi32.LogonUser
        /// </summary>
        /// <param name="lpszUsername"></param>
        /// <param name="lpszDomain"></param>
        /// <param name="lpszPassword"></param>
        /// <param name="dwLogonType"></param>
        /// <param name="dwLogonProvider"></param>
        /// <param name="phToken"></param>
        /// <returns></returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(
            string lpszUsername,
            string lpszDomain,
            string lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            out IntPtr phToken
            );

        private static WindowsIdentity LogonUserTCPListen(string userName, string domain, string password)
        {
            // need a full duplex stream - loopback is easiest way to get that
            TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 0);
            tcpListener.Start();
            ManualResetEvent done = new ManualResetEvent(false);

            WindowsIdentity id = null;
            tcpListener.BeginAcceptTcpClient(delegate(IAsyncResult asyncResult)
            {
                try
                {
                    using (NegotiateStream serverSide = new NegotiateStream(
                         tcpListener.EndAcceptTcpClient(asyncResult).GetStream()))
                    {
                        serverSide.AuthenticateAsServer(CredentialCache.DefaultNetworkCredentials,
                             ProtectionLevel.None, TokenImpersonationLevel.Impersonation);
                        id = (WindowsIdentity)serverSide.RemoteIdentity;
                    }
                }
                catch
                { id = null; }
                finally
                { done.Set(); }
            }, null);

            using (NegotiateStream clientSide = new NegotiateStream(new TcpClient("localhost",
                 ((IPEndPoint)tcpListener.LocalEndpoint).Port).GetStream()))
            {
                try
                {
                    clientSide.AuthenticateAsClient(new NetworkCredential(userName, password, domain),
                     "", ProtectionLevel.None, TokenImpersonationLevel.Impersonation);
                }
                catch
                { id = null; }//When the authentication fails it throws an exception
            }
            tcpListener.Stop();
            done.WaitOne();//Wait until we really have the id populated to continue
            return id;
        }
        #endregion

        #region IDisposable Interface

        /// <summary>
        /// 자원 해제
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 명시적 자원 해제
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            WNetCancelConnection2(_networkName, 0, true);
        }
        #endregion
    }

    /// <summary>
    /// 네트워크 자원 형태의 구조체
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public class NetResource
    {
        /// <summary>
        /// 리소스의 범위
        /// </summary>
        public ResourceScope Scope;
        /// <summary>
        /// 리소스의 형식
        /// </summary>
        public ResourceType ResourceType;
        /// <summary>
        /// 리소스의 표시형식
        /// </summary>
        public ResourceDisplaytype DisplayType;
        /// <summary>
        /// 리소스 사용량
        /// </summary>
        public int Usage;
        /// <summary>
        /// 로컬 이름
        /// </summary>
        public string LocalName;
        /// <summary>
        /// 원격 이름
        /// </summary>
        public string RemoteName;
        /// <summary>
        /// 주석
        /// </summary>
        public string Comment;
        /// <summary>
        /// 제공자
        /// </summary>
        public string Provider;
    }

    /// <summary>
    /// 리소스 범위
    /// </summary>
    public enum ResourceScope : int
    {
        /// <summary>
        /// 연결됨
        /// </summary>
        Connected = 1,
        /// <summary>
        /// 공개네트워크
        /// </summary>
        GlobalNetwork,
        /// <summary>
        /// 기억된
        /// </summary>
        Remembered,
        /// <summary>
        /// 최근
        /// </summary>
        Recent,
        /// <summary>
        /// 컨텍스트
        /// </summary>
        Context
    };

    /// <summary>
    /// 리소스 형식
    /// </summary>
    public enum ResourceType : int
    {
        /// <summary>
        /// Any
        /// </summary>
        Any = 0,
        /// <summary>
        /// Disk
        /// </summary>
        Disk = 1,
        /// <summary>
        /// Print
        /// </summary>
        Print = 2,
        /// <summary>
        /// Reserved
        /// </summary>
        Reserved = 8,
    }

    /// <summary>
    /// 리소스 표시형식
    /// </summary>
    public enum ResourceDisplaytype : int
    {
        /// <summary>
        /// 일반
        /// </summary>
        Generic = 0x0,
        /// <summary>
        /// 도메인
        /// </summary>
        Domain = 0x01,
        /// <summary>
        /// 서버
        /// </summary>
        Server = 0x02,
        /// <summary>
        /// 공유
        /// </summary>
        Share = 0x03,
        /// <summary>
        /// 파일
        /// </summary>
        File = 0x04,
        /// <summary>
        /// 그룹
        /// </summary>
        Group = 0x05,
        /// <summary>
        /// 네트워크
        /// </summary>
        Network = 0x06,
        /// <summary>
        /// 루트
        /// </summary>
        Root = 0x07,
        /// <summary>
        /// 공유관리
        /// </summary>
        Shareadmin = 0x08,
        /// <summary>
        /// 디렉토리
        /// </summary>
        Directory = 0x09,
        /// <summary>
        /// 트리
        /// </summary>
        Tree = 0x0a,
        /// <summary>
        /// ndscontainer
        /// </summary>
        Ndscontainer = 0x0b
    }

    /// <summary>
    /// Logon Type
    /// </summary>
    public enum LogonType
    {
        /// <summary>
        /// This logon type is intended for users who will be interactively using the computer, such as a user being logged on  
        /// by a terminal server, remote shell, or similar process.
        /// This logon type has the additional expense of caching logon information for disconnected operations; 
        /// therefore, it is inappropriate for some client/server applications,
        /// such as a mail server.
        /// </summary>
        LOGON32_LOGON_INTERACTIVE = 2,

        /// <summary>
        /// This logon type is intended for high performance servers to authenticate plaintext passwords.
        /// The LogonUser function does not cache credentials for this logon type.
        /// </summary>
        LOGON32_LOGON_NETWORK = 3,

        /// <summary>
        /// This logon type is intended for batch servers, where processes may be executing on behalf of a user without 
        /// their direct intervention. This type is also for higher performance servers that process many plaintext
        /// authentication attempts at a time, such as mail or Web servers. 
        /// The LogonUser function does not cache credentials for this logon type.
        /// </summary>
        LOGON32_LOGON_BATCH = 4,

        /// <summary>
        /// Indicates a service-type logon. The account provided must have the service privilege enabled. 
        /// </summary>
        LOGON32_LOGON_SERVICE = 5,

        /// <summary>
        /// This logon type is for GINA DLLs that log on users who will be interactively using the computer. 
        /// This logon type can generate a unique audit record that shows when the workstation was unlocked. 
        /// </summary>
        LOGON32_LOGON_UNLOCK = 7,

        /// <summary>
        /// This logon type preserves the name and password in the authentication package, which allows the server to make 
        /// connections to other network servers while impersonating the client. A server can accept plaintext credentials 
        /// from a client, call LogonUser, verify that the user can access the system across the network, and still 
        /// communicate with other servers.
        /// NOTE: Windows NT:  This value is not supported. 
        /// </summary>
        LOGON32_LOGON_NETWORK_CLEARTEXT = 8,

        /// <summary>
        /// This logon type allows the caller to clone its current token and specify new credentials for outbound connections.
        /// The new logon session has the same local identifier but uses different credentials for other network connections. 
        /// NOTE: This logon type is supported only by the LOGON32_PROVIDER_WINNT50 logon provider.
        /// NOTE: Windows NT:  This value is not supported. 
        /// </summary>
        LOGON32_LOGON_NEW_CREDENTIALS = 9,
    }

    /// <summary>
    /// NTLM Logon Provider Version
    /// </summary>
    public enum LogonProvider
    {
        /// <summary>
        /// Use the standard logon provider for the system. 
        /// The default security provider is negotiate, unless you pass NULL for the domain name and the user name 
        /// is not in UPN format. In this case, the default provider is NTLM. 
        /// NOTE: Windows 2000/NT:   The default security provider is NTLM.
        /// </summary>
        LOGON32_PROVIDER_DEFAULT = 0,
        /// <summary>
        /// Windows NT 3.5
        /// </summary>
        LOGON32_PROVIDER_WINNT35 = 1,
        /// <summary>
        /// Windows NT 4.0
        /// </summary>
        LOGON32_PROVIDER_WINNT40 = 2,
        /// <summary>
        /// Windows NT 5.0
        /// </summary>
        LOGON32_PROVIDER_WINNT50 = 3
    }
}
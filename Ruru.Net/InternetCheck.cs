using System.Net;
namespace Ruru.Net
{
    /// <summary>
    /// 인터넷 연결여부 체크
    /// </summary>
    public static class InternetCheck
    {
        // Internet Check
        [System.Runtime.InteropServices.DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);
        
        /// <summary>
        /// 인터넷 연결여부
        /// </summary>
        public static bool IsInternetConnected
        {
            get
            {
                int desc = 0;
                return InternetGetConnectedState(out desc, 0);
            }
        }

        public static IPHostEntry GetLocalIPHost
        {
            get
            {
                return Dns.GetHostEntry(Dns.GetHostName());
            }
        }
    }
}

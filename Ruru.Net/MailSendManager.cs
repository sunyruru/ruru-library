namespace Ruru.Net
{
    using System;
    using System.Net;
    using System.Net.Mail;
    using System.Threading;

    /// <summary>
    /// 메일 발송 도우미
    /// </summary>
    public class MailSendManager
    {
        #region Privates
        private bool _lockState = false;
        private SmtpClient _client = null;
        private AutoResetEvent _lock = null;
        #endregion

        #region Properties
        /// <summary>
        /// 보낸 사람을 인증하는 데 사용되는 자격 증명을 가져오거나 설정합니다.
        /// </summary>
        /// <value>인증에 사용할 자격 증명을 나타내는 System.Net.ICredentialsByHost이거나, 자격 증명이 지정되지 않은 경우 null입니다.</value>
        /// <exception cref="System.InvalidOperationException">전자 메일을 보내는 동안 이 속성 값을 변경한 경우</exception>
        public ICredentialsByHost Credentials { get; set; }

        /// <summary>
        /// 보내는 전자 메일 메시지의 처리 방법을 지정합니다.
        /// </summary>
        /// <value>전자 메일 메시지가 배달되는 방법을 나타내는 System.Net.Mail.SmtpDeliveryMethod입니다.</value>
        public SmtpDeliveryMethod DeliveryMethod { get; set; }

        /// <summary>
        /// System.Net.Mail.SmtpClient에서 SSL(Secure Sockets Layer)을 사용하여 연결을 암호화할지 여부를 지정합니다.
        /// </summary>
        /// <value>System.Net.Mail.SmtpClient에서 SSL을 사용하면 true이고, 그렇지 않으면 false입니다. 기본값은 false입니다.</value>
        public bool EnableSsl { get; set; }

        /// <summary>
        /// SMTP 트랜잭션에 사용되는 호스트의 이름 또는 IP 주소를 가져오거나 설정합니다.
        /// </summary>
        /// <value> SMTP 트랜잭션에 사용할 컴퓨터의 이름 또는 IP 주소가 들어 있는 <see cref="System.String"/>입니다.</value>
        /// <exception cref="System.InvalidOperationException">전자 메일을 보내는 동안 이 속성 값을 변경한 경우</exception>
        /// <exception cref="System.ArgumentException">set 작업에 지정된 값이 System.String.Empty("")와 같은 경우</exception>
        /// <exception cref="System.ArgumentNullException">set 작업에 지정된 값이 null인 경우</exception>
        public string Host { get; set; }

        /// <summary>
        /// 응용 프로그램이 로컬 SMTP 서버에서 처리할 메일 메시지를 저장하는 폴더를 가져오거나 설정합니다.
        /// </summary>
        /// <value>전자 메일 메시지의 픽업 디렉터리를 지정하는 <see cref="System.String"/>입니다.</value>
        public string PickupDirectoryLocation { get; set; }

        /// <summary>
        /// SMTP 트랜잭션에 사용되는 포트를 가져오거나 설정합니다.
        /// </summary>
        /// <value>SMTP 호스트의 포트 번호가 들어 있는 <see cref="System.Int32"/>입니다. 기본값은 25입니다.</value>
        /// <exception cref="System.InvalidOperationException">전자 메일을 보내는 동안 이 속성 값을 변경한 경우</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">set 작업에 지정된 값이 0 이하인 경우</exception>
        public int Port { get; set; }

        /// <summary>
        /// 전자 메일 메시지를 전송하는 데 사용되는 네트워크 연결을 가져옵니다.
        /// </summary>
        /// <value>SMTP에 사용되는 <see cref="Ruru.Net.MailSendManager.Host"/> 속성에 연결하는 <see cref="System.Net.ServicePoint"/>입니다.</value>
        /// <exception cref="System.InvalidOperationException"><see cref="Ruru.Net.MailSendManager.Host"/>가 null이거나 빈 문자열("")인 경우- 또는 -<see cref="Ruru.Net.MailSendManager.Port"/>가 0인 경우</exception>
        public ServicePoint ServicePoint { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TargetName { get; set; }

        /// <summary>
        /// 동기 <see cref="O:Ruru.Net.MailSendManager.SendMail"/> 호출이 완료되어야 하는 제한 시간을 지정하는 값을 가져오거나 설정합니다.
        /// </summary>
        /// <value>제한 시간 값(밀리초)을 지정하는 <see cref="System.Int32"/>입니다. 기본값은 100,000(100초)입니다.</value>
        /// <exception cref="System.InvalidOperationException">전자 메일을 보내는 동안 이 속성 값을 변경한 경우</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">set 작업에 지정된 값이 0보다 작은 경우</exception>
        public int Timeout { get; set; }

        /// <summary>
        /// <see cref="System.Net.CredentialCache.DefaultCredentials"/>를 요청과 함께 보낼지 여부를 제어하는 <see cref="System.Boolean"/> 값을 가져오거나 설정합니다.
        /// </summary>
        /// <value>기본 자격 증명이 사용되면 true이고, 그렇지 않으면 false입니다. 기본값은 false입니다.</value>
        /// <exception cref="System.InvalidOperationException">전자 메일을 보내는 동안 이 속성 값을 변경한 경우</exception>
        public bool UseDefaultCredentials { get; set; }

        #endregion        

        #region 생성자, 소멸자
        /// <summary>
        /// 기본 생성자
        /// </summary>
        public MailSendManager()
            : this("localhost") { }

        /// <summary>
        /// 지정된 호스트에 접속합니다.
        /// </summary>
        /// <param name="sHost">지정된 메일 발신 호스트 <see cref="System.String"/></param>
        public MailSendManager(string sHost)
            : this(sHost, 25) { }

        /// <summary>
        /// 지정된 호스트와 포트에 접속합니다.
        /// </summary>
        /// <param name="sHost">지정된 메일 발신 호스트 <see cref="System.String"/></param>
        /// <param name="iPort">지정된 메일 발신 포트 <see cref="System.Int32"/></param>
        public MailSendManager(string sHost, int iPort)
        {
            try
            {
                _lock = new AutoResetEvent(_lockState);

                this.Host = sHost;
                this.Port = iPort;

                GetConnection();
            }
            catch (System.ArgumentOutOfRangeException) { throw; }
            catch (Exception) { throw; }
        }
        #endregion

        #region Member Methods
		private SmtpClient GetConnection()
        {
            this._client = this._client ?? new SmtpClient(this.Host, this.Port);
            return this._client;
        }

        /// <summary>
        /// 배달용 SMTP 서버로 지정된 메시지를 보냅니다.
        /// </summary>
        /// <param name="message">보낼 메시지가 들어 있는 <see cref="System.Net.Mail.MailMessage"/>입니다.</param>
        /// <returns>성공 여부를 알려주는 <see cref="System.Boolean"/> 값입니다.</returns>
        public bool SendMail(MailMessage message)
        {
            bool bResult = false;
            return bResult;
        }
        /// <summary>
        /// 배달용 SMTP 서버로 지정된 전자 메일 메시지를 보냅니다. 메시지의 보낸 사람, 받는 사람, 제목 및 메시지 본문은 <see cref="System.String"/> 개체를 사용하여 지정됩니다.
        /// </summary>
        /// <param name="from">메시지의 보낸 사람 주소 정보가 들어 있는 <see cref="System.String"/>입니다.</param>
        /// <param name="recipients">메시지를 보낼 대상 주소가 들어 있는 입니다.</param>
        /// <param name="subject">메시지의 제목 줄이 들어 있는 <see cref="System.String"/>입니다.</param>
        /// <param name="body">메시지 본문이 들어 있는 <see cref="System.String"/>입니다.</param>
        /// <returns>성공 여부를 알려주는 <see cref="System.Boolean"/> 값입니다.</returns>
        public bool SendMail(string from, string recipients, string subject, string body)
        {
            bool bResult = false;
            System.Net.Mail.SmtpClient oSMTP = null;

            try
            {
                oSMTP = new System.Net.Mail.SmtpClient("localhost", 25);
                oSMTP.UseDefaultCredentials = false;
                oSMTP.EnableSsl = false;
                oSMTP.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                oSMTP.Credentials = new System.Net.NetworkCredential("sunyruru", "Babo8426!");

                MailAddress sFrom = new MailAddress("sunyruru@ruru.com", "김선우", System.Text.Encoding.UTF8);
                MailAddress to = new MailAddress("sunyruru@onestx.com");

                MailMessage message = new MailMessage(sFrom, to);

                message.Body = "This is a test e-mail message sent by an application. ";
                string someArrows = new string(new char[] { '\u2190', '\u2191', '\u2192', '\u2193' });
                message.Body += Environment.NewLine + someArrows;
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.Subject = string.Format("test message 2{0}", someArrows);
                message.SubjectEncoding = System.Text.Encoding.UTF8;

                try
                {
                    // 비동기로 메일을 보낸다.
                    oSMTP.SendCompleted += new SendCompletedEventHandler(oSMTP_SendCompleted);
                    oSMTP.SendAsync(message, _lockState);
                    _lock.WaitOne(1000);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
            }
            return bResult;
        }

        void oSMTP_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // 오류 발생 Re-Queueing
                throw e.Error;
            }
            else
            {
                // 정상 발송 로그 등록
            }
            _lock.Set();
        }
	    #endregion
    }
}

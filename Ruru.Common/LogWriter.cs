namespace Ruru.Common
{
    using System;
    using System.IO;

    /// <summary>
    /// Log 파일 생성 클래스
    /// </summary>
    public class LogWriter
    {
        private string LogDirectory = "";
        private FileStream oFs = null;
        private StreamWriter oWriter = null;

        /// <summary>
        /// 로그 타입
        /// </summary>
        public enum LogType
        {
            /// <summary>
            /// 사용자
            /// </summary>
            USER,
            /// <summary>
            /// 부서
            /// </summary>
            DEPT
        }

        /// <summary>
        /// 기본 생성자
        /// </summary>
        public LogWriter() : this(System.Environment.CurrentDirectory)
        {
        }

        /// <summary>
        /// 로그 파일 경로 지정 생성자
        /// </summary>
        /// <param name="sLogPath">로그 파일 경로</param>
        public LogWriter(string sLogPath) : this(sLogPath, string.Empty)
        {
        }

        /// <summary>
        /// 로그 파일 경로, 프로젝트 폴더명 지정 생성자
        /// </summary>
        /// <param name="sLogPath">로그 파일 경로</param>
        /// <param name="sProjectName">프로젝트 폴더명</param>
        public LogWriter(string sLogPath, string sProjectName)
        {
            this.LogDirectory = Path.Combine(sLogPath, "LOG");

            if (!string.IsNullOrEmpty(sProjectName))
            {
                this.LogDirectory = Path.Combine(this.LogDirectory, sProjectName);
            }

            if (!Directory.Exists(this.LogDirectory))   // 해당 디렉토리가 없으면 만든다.
            {
                Directory.CreateDirectory(this.LogDirectory);
            }

            string strFullPath = string.Format(@"{0}\{1}.LOG", this.LogDirectory, DateTime.Now.ToString("yyyy-MM-dd"));

            //oFs = new FileStream(GetFileName(strFullPath), FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
            oFs = new FileStream(strFullPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);

            //oWriter = new StreamWriter(oFs, System.Text.Encoding.GetEncoding("euc-kr"));
            oWriter = new StreamWriter(oFs, System.Text.Encoding.UTF8);
        }    

        /// <summary>
        /// 로그파일에 메시지 작성
        /// </summary>
        /// <param name="sMessage">메시지</param>
        public void WriteLog(string sMessage)
        {
            try
            {
                string.Format("[{0}] {1}", DateTime.Now, sMessage);
                oWriter.WriteLine(sMessage);
            }
            catch
            {
                //Do Nothing!!
            }
            finally
            {
                //if (oWriter != null) oWriter.Close();
                //if (oFs != null) oFs.Close();
            }
        }

        /// <summary>
        /// 파일 경로의 파일을 읽음
        /// </summary>
        /// <param name="strPath">파일 경로</param>
        /// <returns>파일 내용</returns>
        private string GetFileName(string strPath)
        {
            return GetFileName(strPath, 0);
        }

        /// <summary>
        /// 파일 경로의 파일을 지정 위치부터 읽음
        /// </summary>
        /// <param name="strPath">파일 경로</param>
        /// <param name="idx">파일 커서 위치</param>
        /// <returns>파일 내용</returns>
        private string GetFileName(string strPath, int idx)
        {
            if (idx == 1) strPath = strPath.Replace(".LOG", string.Format("({0}).LOG", idx.ToString()));
            else if (idx > 0) strPath = strPath.Replace(string.Format("({0}).LOG", (idx - 1).ToString()), string.Format("({0}).LOG", idx.ToString()));

            if (File.Exists(strPath))
            {
                return this.GetFileName(strPath, (idx + 1));
            }

            return strPath;
        }

        /// <summary>
        /// 사용 Unmanaged 자원의 해제
        /// </summary>
        public void Dispose()
        {
            if (oWriter != null) oWriter.Close();
            if (oFs != null) oFs.Close();
            System.GC.SuppressFinalize(this);
        }
    }
}

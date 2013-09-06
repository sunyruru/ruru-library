namespace Ruru.Common
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Windows System의 Colsole 명령어를 실행할 수 있도록 함
    /// </summary>
    public class ConsoleUtil
    {
        /// <summary>
        /// Windows System의 Console 명령어와 인자를 받아 명령어 실행 후 결과를 반환
        /// </summary>
        /// <param name="FileName"><see cref="System.String"/> 실행 파일명</param>
        /// <param name="Argments"><see cref="System.String"/> 명령어 인자</param>
        /// <returns><see cref="System.String"/> 명령 실행 결과</returns>
        public string Run(string FileName, string Argments)
        {
            string result = string.Empty;
            Process p = new Process();

            try
            {
                p.StartInfo.FileName = FileName;
                p.StartInfo.Arguments = Argments;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.Start();

                result = p.StandardOutput.ReadToEnd();

                p.WaitForExit();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (p != null)
                {
                    p.Dispose();
                    p = null;
                }
            }

            return result;
        }
    }
}

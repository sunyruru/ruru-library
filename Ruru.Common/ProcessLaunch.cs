namespace Ruru.Common
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;

    /// <summary>
    /// 보이지 않게 Background로 Process 실행을 위한 클래스
    /// </summary>
    public class ProcessLaunch
    {
        /// <summary>
        /// 명령 실행 인자값을 설정합니다.
        /// </summary>
        public string Args
        {
            get { return m_args; }
            set { m_args = value; }
        }
        private string m_args;
        
        /// <summary>
        /// 오류 메시지를 확인합니다.
        /// </summary>
        public string ErrorMessage
        {
            get { return m_errorMessage; }
            set { m_errorMessage = value; }
        }
        private string m_errorMessage;
        
        /// <summary>
        /// 실행할 파일명을 지정합니다.
        /// </summary>
        public string Filename
        {
            get { return m_filename; }
            set { m_filename = value; }
        }
        private string m_filename;
        
        /// <summary>
        /// 프로그램 실행 후 반환되는 결과값을 가집니다.
        /// </summary>
        public string ProcessOutput
        {
            get { return m_processOutput; }
            set { m_processOutput = value; }
        }
        private string m_processOutput;
        
        /// <summary>
        /// ProgramFiles 디렉토리가 지정됩니다.
        /// </summary>
        public string ProgramFilesDir
        {
            get { return m_programFilesDir; }
        }
        private string m_programFilesDir;
        
        /// <summary>
        /// 실행 시 Console Shell 창을 사용할지 여부 지정
        /// </summary>
        public bool UseShellExecute
        {
            get { return m_useShellExecute; }
            set { m_useShellExecute = value; }
        }
        private bool m_useShellExecute = true;
        
        /// <summary>
        /// 프로세스 창 스타일을 지정
        /// </summary>
        public ProcessWindowStyle WindowStyle
        {
            get { return m_windowStyle; }
            set { m_windowStyle = value; }
        }
        private ProcessWindowStyle m_windowStyle;
        
        /// <summary>
        /// 프로세스의 작업 디렉토리 지정
        /// </summary>
        public string WorkingDir
        {
            get { return m_workingDir; }
            set { m_workingDir = value; }
        }
        private string m_workingDir;

        private ProcessStartInfo m_startInfo = new ProcessStartInfo();

        /// <summary>
        /// 기본 생성자
        /// </summary>
        public ProcessLaunch()
        {
            this.GetEnvironmentVars();
            this.m_workingDir = this.m_programFilesDir;
        }

        private void GetEnvironmentVars()
        {
            m_programFilesDir = Environment.GetEnvironmentVariable("ProgramFiles");
        }

        /// <summary>
        /// 프로세스 실행
        /// </summary>
        public void RunProcess()
        {
            this.m_startInfo.FileName = this.m_filename;
            this.m_startInfo.Arguments = this.m_args;
            this.m_startInfo.WindowStyle = this.m_windowStyle;
            this.m_startInfo.UseShellExecute = this.m_useShellExecute;
            this.m_startInfo.WorkingDirectory = this.m_workingDir;
            this.m_startInfo.RedirectStandardOutput = true;
            this.m_startInfo.CreateNoWindow = true;
            try
            {
                this.m_processOutput = "";
                Process process = new Process();
                process.StartInfo = this.m_startInfo;
                process.Start();
                this.m_processOutput = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
            }
            catch (Win32Exception exception)
            {
                this.m_errorMessage = " Caught a Win32Exception. Error = " + exception.Message;
            }
            catch (InvalidOperationException)
            {
                this.m_errorMessage = " Caught a InvalidOperationException Error.";
            }
        }
    }
}

namespace Ruru.Common.Config
{
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// INI 설정파일을 사용할 수 있도록 합니다.
    /// </summary>
    public class INIParser
    {
        private string path;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(
            string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(
            string section, string key, string def,
            StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// INI 파일의 경로를 지정합니다.
        /// </summary>
        /// <param name="INIPath">INI 파일 경로</param>
        public INIParser(string INIPath)
        {
            path = INIPath;
        }

        /// <summary>
        /// INI 파일에 값을 입력
        /// </summary>
        /// <param name="Section">섹션</param>
        /// <param name="Key">키</param>
        /// <param name="Value">값</param>
        public void WriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.path);
        }

        /// <summary>
        /// INI 파일의 정보를 읽음
        /// </summary>
        /// <param name="Section">섹션</param>
        /// <param name="Key">키</param>
        /// <returns>값</returns>
        public string ReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(
                Section, Key, string.Empty, temp, 255, this.path);
            return temp.ToString();
        }
    }
}
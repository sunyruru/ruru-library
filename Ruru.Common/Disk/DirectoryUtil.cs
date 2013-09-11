namespace Ruru.Common.Disk
{
    using System;
    using System.IO;

    public class DirectoryUtil
    {
        // 디렉토리 생성
        public static bool MakeDirectory(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;

            }

            return true;
        }

        // 디렉토리 삭제
        public static bool DeleteDirectory(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.Delete(path);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;

            }

            return true;
        }

    }
}

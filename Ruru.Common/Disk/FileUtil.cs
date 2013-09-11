namespace Ruru.Common.Disk
{
    using System;
    using System.IO;
    using System.Text;

    public class FileUtil
    {       
        // 파일생성
        public static void WriteStringToFile(string source, string filePhysicalPath)
        {
            StreamWriter swFile = null;
            
            try
            {
                if (System.IO.File.Exists(filePhysicalPath))
                {
                    System.IO.File.Delete(filePhysicalPath);
                }

                System.IO.File.WriteAllText(filePhysicalPath, source, Encoding.Default);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (swFile != null) swFile.Close();
            }
        }
        public static void WriteStringToFile(string source, string filePhysicalPath, Encoding encType)
        {
            StreamWriter swFile = null;

            try
            {
                if (System.IO.File.Exists(filePhysicalPath))
                {
                    System.IO.File.Delete(filePhysicalPath);
                }

                System.IO.File.WriteAllText(filePhysicalPath, source, encType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (swFile != null) swFile.Close();
            }
        }
        
        // 파일이동
        public static void MoveFile(string OldFullPath, string TargetFullPath)
        {
            System.IO.File.Move (OldFullPath, TargetFullPath);
        }

        // 파일복사
        public static void CopyFile(string OldFullPath, string TargetFullPath)
        {
            System.IO.File.Copy(OldFullPath, TargetFullPath);
        }

        // 파일크기 숫자로 얻기(Byte)      
        public static long GetFileSize(string filepath)
        {
            if (!System.IO.File.Exists(filepath))
            {
                return 0;
            }

            System.IO.FileInfo fi = new System.IO.FileInfo(filepath);
            return fi.Length;
        }

        // 파일크기 문자로 얻기(ex: 500MB)
        public static string GetFileSizeToString(string filepath)
        {
            string strSize = string.Empty;

            long fileLength = GetFileSize(filepath);
            
            try
            {
                if (fileLength > 1024 * 1024 * 1024)
                {
                    strSize = Convert.ToString(fileLength / (1024 * 1024 * 1024)) + "GB";
                }
                else if (fileLength > 1024 * 1024)
                {
                    strSize = Convert.ToString(fileLength / (1024 * 1024)) + "MB";
                }
                else if (fileLength > 1024)
                {
                    strSize = Convert.ToString(fileLength / (1024)) + "KB";
                }
                else
                {
                    strSize = Convert.ToString(fileLength) + "B";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strSize;
        }
    }
}

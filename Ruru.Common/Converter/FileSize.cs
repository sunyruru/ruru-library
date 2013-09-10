namespace Ruru.Common.Converter
{
    /// <summary>
    /// 파일 사이즈를 간편하게 표시해주는 클래스
    /// </summary>
    /// <example>
    /// FileSize.GetSize(1024L);
    /// Expect return string : "1KB"
    /// </example>
    public sealed class FileSize : ConvertBase
    {
        /// <summary>
        /// Size 단위
        /// </summary>
        public enum UNITS 
        {
            /// <summary>
            /// Byte
            /// </summary>
            B,
            /// <summary>
            /// KB
            /// </summary>
            KB,
            /// <summary>
            /// MB
            /// </summary>
            MB,
            /// <summary>
            /// GB
            /// </summary>
            GB,
            /// <summary>
            /// TB
            /// </summary>
            TB
        }

        double b = 0;
        double kb = 0;
        double mb = 0;
        double gb = 0;
        double tb = 0;

        /// <summary>
        /// Byte Size
        /// </summary>
        public double B
        {
            get
            {
                return this.b;
            }
            set
            {
                this.b = value;
                this.kb = this.b / 1024;
                this.mb = this.kb / 1024;
                this.gb = this.mb / 1024;
                this.tb = this.gb / 1024;
            }
        }

        /// <summary>
        /// KB Size
        /// </summary>
        public double KB
        {
            get
            {
                return this.kb;
            }
            set
            {
                this.kb = value;
                this.B = this.kb * 1024;
            }
        }

        /// <summary>
        /// MB Size
        /// </summary>
        public double MB
        {
            get
            {
                return this.mb;
            }
            set
            {
                this.mb = value;
                this.KB = this.mb * 1024;
            }
        }

        /// <summary>
        /// GB Size
        /// </summary>
        public double GB
        {
            get
            {
                return this.gb;
            }
            set
            {
                this.gb = value;
                this.MB = this.gb * 1024;
            }
        }

        /// <summary>
        /// TB Size
        /// </summary>
        public double TB
        {
            get
            {
                return this.tb;
            }
            set
            {
                this.tb = value;
                this.GB = this.tb * 1024;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public FileSize()
            : this(0D)
        {
        }

        /// <summary>
        /// Size를 지정한 Constructor
        /// </summary>
        /// <param name="size"></param>
        public FileSize(double size)
        {
            this.B = size;
        }

        /// <summary>
        /// 사이즈를 지정하여 그에 해당하는 가장 짧은 파일 크기 문자열 반환
        /// </summary>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.FormatException"></exception>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string GetSize(double size)
        {
            string sResult = string.Empty;

            try
            {
                sResult = new FileSize(size).ToString();
            }
            catch (System.ArgumentNullException ex) { throw ex; }
            catch (System.FormatException ex) { throw ex; }
            catch { throw; }

            return sResult;
        }

        /// <summary>
        /// 파일사이즈의 가장 짧은 문자열로 반환
        /// </summary>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.FormatException"></exception>
        /// <returns></returns>
        public override string ToString()
        {
            string ret = string.Empty;

            try
            {
                if (b < 1024D)
                    ret = string.Format("{0:0.0}{1}", b, UNITS.B.ToString());
                else if (kb < 1024D)
                    ret = string.Format("{0:0.0}{1}", kb, UNITS.KB.ToString());
                else if (mb < 1024D)
                    ret = string.Format("{0:0.0}{1}", mb, UNITS.MB.ToString());
                else if (gb < 1024D)
                    ret = string.Format("{0:0.0}{1}", gb, UNITS.GB.ToString());
                else
                    ret = string.Format("{0:0.0}{1}", tb, UNITS.TB.ToString());
            }
            catch (System.ArgumentNullException ex) { throw ex; }
            catch (System.FormatException ex) { throw ex; }
            catch { throw; }

            return ret;
        }
    }
}

namespace Ruru.Common.Converter
{
    using System;
    using System.Text;

    public partial class Base64Convert : ConvertBase
    {
        #region " Base64Encode "
        public static string Base64Encode(string src, Encoding enc)
        {
            byte[] arr = null;

            try
            {
                if (enc == null) enc = Encoding.UTF8;
                arr = enc.GetBytes(src);
            }
            catch
            {
                throw;
            }
            return Convert.ToBase64String(arr);
        }
        #endregion

        #region " Base64Decode "
        public static string Base64Decode(string src)
        {
            string sReturn = "";

            if (src != "")
            {
                byte[] arr = null;
                UTF8Encoding uniEnc = null;

                try
                {
                    uniEnc = new UTF8Encoding();
                    arr = Convert.FromBase64String(src);
                    sReturn = uniEnc.GetString(arr);
                }
                catch
                {
                }
                finally
                {
                    uniEnc = null;
                }
            }
            return sReturn;
        }
        #endregion
    }
}

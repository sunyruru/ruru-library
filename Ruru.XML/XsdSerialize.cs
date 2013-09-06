namespace Ruru.XML
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    public static class XsdSerialize
    {
        /// <summary>
        /// Xsd 형식에 따르는 XML 파일을 불러와 해당하는 Class 형식으로 반환합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sXML"></param>
        /// <returns></returns>
        public static T SerializeXsd<T>(string sXML) where T : new()
        {
            T oResult = new T();

            try
            {
                XmlSerializer xSerializer = new XmlSerializer(typeof(T));
                oResult = (T)xSerializer.Deserialize(XmlReader.Create(new StringReader(sXML)));
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return oResult;
        }

        /// <summary>
        /// Xsd 선언된 Class 형식을 문자열 형태로 반환합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oT"></param>
        /// <returns></returns>
        public static string DeserializeXsd<T>(T oT)
        {
            XmlSerializer oXmlSerial = null;
            StringBuilder sb = null;
            StringWriter sw = null;

            try
            {
                sb = new StringBuilder();
                sw = new StringWriter(sb);
                //sw.NewLine = string.Empty;

                oXmlSerial = new XmlSerializer(typeof(T));
                oXmlSerial.Serialize(sw, oT);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Dispose();
                    sw = null;
                }
            }

            return sb.ToString();
        }
    }
}

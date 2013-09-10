namespace Ruru.Common.Converter
{
    using System;
    using System.Data;
    using System.Text;
    using System.Web;

    /// <summary>
    /// XML Convert Class
    /// </summary>
    public partial class XmlConvert : ConvertBase
    {
        #region " GetXmlTypeData From DataTable "
        private static string GetXmlTypeData(DataTable _dtTable)
        {
            string sTmpColumnName = string.Empty;
            string sCol = string.Empty;

            StringBuilder sbXML = new StringBuilder();

            DataColumn dcCol = null;
            DataRow drRow = null;

            try
            {
                sbXML.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                sbXML.Append("<ITEM>");

                sbXML.Append("<HEADER>");
                sbXML.Append("<DEL></DEL>");
                foreach (DataColumn tempLoopVar_dcCol in _dtTable.Columns)
                {
                    dcCol = tempLoopVar_dcCol;
                    sTmpColumnName = dcCol.ColumnName;
                    sbXML.Append("<" + HttpUtility.HtmlEncode(sTmpColumnName) + "></" + HttpUtility.HtmlEncode(sTmpColumnName) + ">");
                }
                sbXML.Append("</HEADER>");

                foreach (DataRow tempLoopVar_drRow in _dtTable.Rows)
                {
                    drRow = tempLoopVar_drRow;
                    sbXML.Append("<ROW>");
                    foreach (DataColumn tempLoopVar_dcCol in _dtTable.Columns)
                    {
                        dcCol = tempLoopVar_dcCol;
                        sTmpColumnName = dcCol.ColumnName;

                        if (drRow[dcCol] is System.DBNull)
                        {
                            sCol = "";
                        }
                        else
                        {
                            sCol = HttpUtility.HtmlEncode(drRow[dcCol].ToString().Trim());
                        }

                        if (dcCol.ColumnName.IndexOf("_DATE") >= 0)
                        {
                            sCol = string.Format("{0:d}", Convert.ToDateTime(sCol));
                        }
                        sbXML.Append("<" + HttpUtility.HtmlEncode(dcCol.ColumnName) + ">" + sCol + "</" + HttpUtility.HtmlEncode(dcCol.ColumnName) + ">");
                    }
                    sbXML.Append("</ROW>");
                }

                sbXML.Append("</ITEM>");
            }
            catch (NullReferenceException)
            {
                sbXML = new StringBuilder();
                sbXML.AppendFormat("<ERROR>TRUE</ERROR>");
            }
            catch (Exception err)
            {
                throw err;
            }
            finally
            {
                if (drRow != null)
                {
                    drRow = null;
                }
                if (dcCol != null)
                {
                    dcCol = null;
                }
            }
            return sbXML.ToString();
        }
        #endregion
    }
}

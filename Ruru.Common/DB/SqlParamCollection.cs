namespace Ruru.Common.DB
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Text;

    public class SqlParamCollection : List<SqlParameter>
    {
        public SqlParameter Add(string parameterName, object value)
        {
            return this.Add(parameterName, value, false);
        }

        public SqlParameter Add(string parameterName, object value, bool isOutput)
        {
            SqlParameter p = new SqlParameter(parameterName, value);
            if (isOutput)
            {
                p.Direction = System.Data.ParameterDirection.Output;
            }
            base.Add(p);
            return p;
        }

        public SqlParameter Add(string parameterName, object value, SqlDbType dbType)
        {
            return this.Add(parameterName, value, dbType, false);
        }

        public SqlParameter Add(string parameterName, object value, SqlDbType dbType, bool isOutput)
        {
            SqlParameter p = this.Add(parameterName, value, isOutput);
            p.SqlDbType = dbType;
            return p;
        }
    }
}

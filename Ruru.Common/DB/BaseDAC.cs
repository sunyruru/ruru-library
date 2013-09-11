namespace Ruru.Common.DB
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using Ruru.Common.Config;
    using Ruru.Common.Exceptions;

    public abstract class BaseDAC : IDisposable
    {
        #region 초기화

        string _connectionString;
        SqlConnection _connection;

        /// <summary>
        /// 'VPD' ConnectionString으로 연결을 생성한다.
        /// </summary>
        public BaseDAC()
        {
            _connectionString = ConfigReader.GetString("NPEX.Frameworks", "ConnectionStrings", "NeoDoQ");

            _connection = new SqlConnection(_connectionString);
            if (_connection != null)
            {
                _connection.Open();
            }
        }

        /// <summary>
        /// 인수로 받은 ConnectionString으로 연결을 생성한다.
        /// </summary>
        /// <param name="connectionString"></param>
        public BaseDAC(string connectionString)
        {
            _connectionString = connectionString;

            _connection = new SqlConnection(_connectionString);
            if (_connection != null)
            {
                _connection.Open();
            }
        }

        #endregion

        #region 프로퍼티

        /// <summary>
        /// 현재 연결된 SqlConnection을 반환
        /// </summary>
        protected SqlConnection Connection
        {
            get
            {
                return _connection;
            }
        }

        /// <summary>
        /// 현재 연결된 Connection String을 반환
        /// </summary>
        protected string ConnectionString
        {
            get
            {
                return _connectionString;
            }
        }

        #endregion

        #region IDisposable 멤버

        /// <summary>
        /// Connection을 해제한다.
        /// </summary>
        public virtual void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }

        #endregion

        #region ExecuteDataSet 메소드 (SELECT)

        /// <summary>
        /// SELECT 수행.
        /// </summary>
        /// <param name="commandType">CommandType</param>
        /// <param name="commandText">SQL 혹은 프로시저명</param>
        /// <returns>데이터셋</returns>
        protected DataSet ExecuteDataSet(CommandType commandType, string commandText)
        {
            return this.ExecuteDataSet(commandType, commandText, 0, 0, null);
        }

        /// <summary>
        /// SELECT 수행.
        /// </summary>
        /// <param name="commandType">CommandType</param>
        /// <param name="commandText">SQL 혹은 프로시저명</param>
        /// <param name="parameters">SqlParameter 배열</param>
        /// <returns>데이터셋</returns>
        protected DataSet ExecuteDataSet(CommandType commandType, string commandText, params SqlParameter[] parameters)
        {
            return this.ExecuteDataSet(commandType, commandText, 0, 0, parameters);
        }

        /// <summary>
        /// SELECT 수행.
        /// </summary>
        /// <param name="commandType">CommandType</param>
        /// <param name="commandText">SQL 혹은 프로시저명</param>
        /// <param name="pageIndex">페이지 인덱스 (0 base)</param>
        /// <param name="pageSize">페이지 당 Row 개수</param>
        /// <param name="parameters">SqlParameter 배열</param>
        /// <returns>데이터셋</returns>
        protected DataSet ExecuteDataSet(CommandType commandType, string commandText, int pageIndex, int pageSize, params SqlParameter[] parameters)
        {
            try
            {
                DataSet ds = null;

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = this.Connection;
                    command.CommandType = commandType;
                    command.CommandText = commandText;

                    if (parameters != null)
                    {
                        foreach (SqlParameter p in parameters)
                        {
                            if (p != null)
                            {
                                if ((p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Input)
                                    && (p.Value == null))
                                {
                                    p.Value = DBNull.Value;
                                }
                                command.Parameters.Add(p);
                            }
                        }
                    }

                    using (SqlDataAdapter da = new SqlDataAdapter(command))
                    {
                        ds = new DataSet();

                        if (pageSize == 0)
                        {
                            da.Fill(ds);
                        }
                        else
                        {
                            da.Fill(ds, (pageSize * pageIndex), pageSize, "Table");
                        }
                    }
                }

                return ds;
            }
            catch (Exception ex)
            {
                // 오류 데이터 추가
                LogManager.AddExceptionData(ref ex, "CommandText", commandText);
                foreach (SqlParameter p in parameters)
                {
                    LogManager.AddExceptionData(ref ex, p.ParameterName, p.Value);
                }

                // 오류 반환
                throw;
            }
        }

        #endregion

        #region ExecuteQuery 메소드 (SELECT)

        /// <summary>
        /// SELECT 수행.
        /// </summary>
        /// <param name="commandType">CommandType</param>
        /// <param name="commandText">SQL 혹은 프로시저명</param>
        /// <returns>데이터 테이블</returns>
        protected DataTable ExecuteQuery(CommandType commandType, string commandText)
        {
            return this.ExecuteQuery(commandType, commandText, 0, 0, null);
        }

        /// <summary>
        /// SELECT 수행.
        /// </summary>
        /// <param name="commandType">CommandType</param>
        /// <param name="commandText">SQL 혹은 프로시저명</param>
        /// <param name="parameters">SqlParameter 배열</param>
        /// <returns>데이터 테이블</returns>
        protected DataTable ExecuteQuery(CommandType commandType, string commandText, params SqlParameter[] parameters)
        {
            return this.ExecuteQuery(commandType, commandText, 0, 0, parameters);
        }

        /// <summary>
        /// SELECT 수행.
        /// </summary>
        /// <param name="commandType">CommandType</param>
        /// <param name="commandText">SQL 혹은 프로시저명</param>
        /// <param name="pageIndex">페이지 인덱스 (0 base)</param>
        /// <param name="pageSize">페이지 당 Row 개수</param>
        /// <param name="parameters">SqlParameter 배열</param>
        /// <returns>데이터셋</returns>
        protected DataTable ExecuteQuery(CommandType commandType, string commandText, int pageIndex, int pageSize, params SqlParameter[] parameters)
        {
            try
            {
                DataTable dt = null;

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = this.Connection;
                    command.CommandType = commandType;
                    command.CommandText = commandText;

                    if (parameters != null)
                    {
                        foreach (SqlParameter p in parameters)
                        {
                            if (p != null)
                            {
                                if ((p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Input)
                                    && (p.Value == null))
                                {
                                    p.Value = DBNull.Value;
                                }
                                command.Parameters.Add(p);
                            }
                        }
                    }

                    using (SqlDataAdapter da = new SqlDataAdapter(command))
                    {
                        dt = new DataTable();

                        if (pageSize == 0)
                        {
                            da.Fill(dt);
                        }
                        else
                        {
                            da.Fill((pageSize * pageIndex), pageSize,dt);
                        }
                    }
                }

                return dt;
            }
            catch (Exception ex)
            {
                // 오류 데이터 추가
                LogManager.AddExceptionData(ref ex, "CommandText", commandText);
                foreach (SqlParameter p in parameters)
                {
                    LogManager.AddExceptionData(ref ex, p.ParameterName, p.Value);
                }

                // 오류 반환
                throw;
            }
        }

        #endregion

        #region ExecuteNonQuery 메소드 (INSERT/UPDATE/DELETE)

        /// <summary>
        /// INSERT/UPDATE/DELETE 수행.
        /// </summary>
        /// <param name="commandType">CommandType</param>
        /// <param name="commandText">SQL 혹은 프로시저명</param>
        /// <returns>영향 받은 Row 개수</returns>
        protected int ExecuteNonQuery(CommandType commandType, string commandText)
        {
            return this.ExecuteNonQuery(commandType, commandText, 0, null);
        }

        /// <summary>
        /// INSERT/UPDATE/DELETE 수행.
        /// </summary>
        /// <param name="commandType">CommandType</param>
        /// <param name="commandText">SQL 혹은 프로시저명</param>
        /// <param name="parameters">SqlParameter 배열</param>
        /// <returns>영향 받은 Row 개수</returns>
        protected int ExecuteNonQuery(CommandType commandType, string commandText, params SqlParameter[] parameters)
        {
            return this.ExecuteNonQuery(commandType, commandText, 0, parameters);
        }

        /// <summary>
        /// INSERT/UPDATE/DELETE 수행.
        /// </summary>
        /// <param name="commandType">CommandType</param>
        /// <param name="commandText">SQL 혹은 프로시저명</param>
        /// <param name="timeOut">타임아웃</param>
        /// <param name="parameters">SqlParameter 배열</param>
        /// <returns>영향 받은 Row 개수</returns>
        protected int ExecuteNonQuery(CommandType commandType, string commandText, int timeOut, params SqlParameter[] parameters)
        {
            try
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = this.Connection;
                    command.CommandType = commandType;
                    command.CommandText = commandText;
                    if (timeOut > 0) command.CommandTimeout = timeOut;

                    if (parameters != null)
                    {
                        foreach (SqlParameter p in parameters)
                        {
                            if (p != null)
                            {
                                if ((p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Input)
                                    && (p.Value == null))
                                {
                                    p.Value = DBNull.Value;
                                }
                                command.Parameters.Add(p);
                            }
                        }
                    }

                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // 오류 데이터 추가
                LogManager.AddExceptionData(ref ex, "CommandText", commandText);
                foreach (SqlParameter p in parameters)
                {
                    LogManager.AddExceptionData(ref ex, p.ParameterName, p.Value);
                }

                // 오류 반환
                throw;
            }
        }

        #endregion

        #region ExecuteScalar 메소드

        /// <summary>
        /// 쿼리를 실행하고 쿼리에서 반환된 결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다. 추가 열이나 행은 무시됩니다.
        /// </summary>
        /// <param name="commandType">CommandType</param>
        /// <param name="commandText">SQL 혹은 프로시저명</param>
        /// <returns>결과 집합의 첫 번째 행의 첫 번째 열</returns>
        protected object ExecuteScalar(CommandType commandType, string commandText)
        {
            return this.ExecuteScalar(commandType, commandText, null);
        }

        /// <summary>
        /// 쿼리를 실행하고 쿼리에서 반환된 결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다. 추가 열이나 행은 무시됩니다.
        /// </summary>
        /// <param name="commandType">CommandType</param>
        /// <param name="commandText">SQL 혹은 프로시저명</param>
        /// <param name="parameters">SqlParameter 배열</param>
        /// <returns>결과 집합의 첫 번째 행의 첫 번째 열</returns>
        protected object ExecuteScalar(CommandType commandType, string commandText, params SqlParameter[] parameters)
        {
            try
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = this.Connection;
                    command.CommandType = commandType;
                    command.CommandText = commandText;

                    if (parameters != null)
                    {
                        foreach (SqlParameter p in parameters)
                        {
                            if (p != null)
                            {
                                if ((p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Input)
                                    && (p.Value == null))
                                {
                                    p.Value = DBNull.Value;
                                }
                                command.Parameters.Add(p);
                            }
                        }
                    }

                    return command.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                // 오류 데이터 추가
                LogManager.AddExceptionData(ref ex, "CommandText", commandText);
                foreach (SqlParameter p in parameters)
                {
                    LogManager.AddExceptionData(ref ex, p.ParameterName, p.Value);
                }

                // 오류 반환
                throw;
            }
        }

        #endregion
    }}

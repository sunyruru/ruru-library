namespace Ruru.Common
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Data.SqlClient;
    using System.Text;

    /// <summary>
    /// Database 관련 Class
    /// </summary>
    public class DBManager : IDisposable
    {
        private SqlConnection _conn = null;
        private Exception _Exception = null;
        private String _ConnectionString = "";
        private Hashtable _htParams = null;

        enum DiffLocation
        {
            First,
            Second
        }

        #region "Properties ..."
        /// <summary>
        /// MS-SQL 연결문자열 입니다.
        /// </summary>
        public string ConnectionString { get; set;}

        /// <summary>
        /// DBManager 에서 발생한 가장 마지막 Exception을 반환합니다.
        /// </summary>
        public Exception CurrentError { get; set; }
        #endregion

        
        #region " Constructor , Destructor"
        /// <summary>
        /// Class Constructor
        /// </summary>
        public DBManager()
        {
            // Class Constructor
            this._htParams = new Hashtable();
        }

        /// <summary>
        /// DBManager Constructor
        /// </summary>
        /// <param name="strConnection">MS-SQL 연결 문자열</param>
        public DBManager(string strConnection)
        {
            try
            {
                this._conn = new SqlConnection(strConnection);
                this._htParams = new Hashtable();
            }
            catch (Exception ex)
            {
                this._Exception = ex;
                this._conn = null;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// DataBase Connection 이 null 이고 Connection String 이 있는 경우만 새로운 Connection을 생성합니다.
        /// </summary>
        public void Connect()
        {
            if (this._conn != null && this._ConnectionString != "")
            {
                try
                {
                    this._conn = new SqlConnection(this._ConnectionString);
                }
                catch (Exception ex)
                {
                    this._Exception = ex;
                    this._conn = null;
                }
            }
        }

        /// <summary>
        /// Insert 혹은 Update 아이템 등록 
        /// </summary>
        /// <param name="ColumnName">아이템명</param>
        /// <param name="value">아이템값</param>
        public void AddItem(string ColumnName, object value)
        {
            try
            {
                this._htParams.Add(ColumnName, value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 등록 아이템을 초기화
        /// </summary>
        public void ClearItem()
        {
            this._htParams.Clear();
        }

        /// <summary>
        /// Insert 혹은 Update 아이템 삭제
        /// </summary>
        /// <param name="ColumnName">제거할 아이템명</param>
        public void RemoveItem(string ColumnName)
        {
            try
            {
                this._htParams.Remove(ColumnName);
            }
            catch
            {
                throw new Exception("선택한 아이템이 없습니다.");
            }
        }

        /// <summary>
        /// Insert 전 반드시 AddItem() 호출로 insert할 아이템을 추가 시켜줘야 합니다.
        /// </summary>
        /// <param name="TBL">대상 테이블명</param>
        /// <returns>영향받은 행의 수</returns>
        public int Insert(string TBL)
        {
            string query = "";
            int InsertedRow = 0;
            SqlCommand comm = null;
            StringBuilder s1 = new StringBuilder();
            StringBuilder s2 = new StringBuilder();

            // Insert 컬럼과 값이 셋팅 되지 않은 경우..
            if (this._htParams == null)
            {
                this._Exception = new Exception("No Item");
                return 0;
            }

            this._Exception = null;

            try
            {
                IDictionaryEnumerator enumInterface = this._htParams.GetEnumerator();

                bool first = true;
                // Insert 쿼리 구문 만들기
                while (enumInterface.MoveNext())
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        s1.Append(", ");
                        s2.Append(", ");
                    }
                    s1.Append(enumInterface.Key.ToString());
                    s2.Append(string.Format("@{0}", enumInterface.Key.ToString()));
                }

                query = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", TBL, s1, s2);

                comm = new SqlCommand(query, this._conn);

                // Parameter 추가
                enumInterface = this._htParams.GetEnumerator();
                while (enumInterface.MoveNext())
                {
                    comm.Parameters.Add(new SqlParameter(string.Format("@{0}", enumInterface.Key.ToString()), enumInterface.Value.ToString()));
                }

                if (this._conn.State != ConnectionState.Open) this._conn.Open();

                InsertedRow = comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                this._Exception = ex;
            }
            finally
            {
                comm.Dispose();
            }

            return InsertedRow;
        }

        /// <summary>
        /// Update 전 반드시 AddItem() 호출로 update할 아이템을 추가 시켜줘야 합니다.
        /// </summary>
        /// <param name="TBL">대상 테이블명</param>
        /// <param name="KeyName">업데이트 키 컬럼명</param>
        /// <param name="KeyValue">업데이트 키 값</param>
        /// <returns>영향받은 행의 수</returns>
        public int Update(string TBL, string KeyName, string KeyValue)
        {
            string query = "";
            int UpdatedRow = 0;
            SqlCommand comm = null;
            StringBuilder s1 = new StringBuilder();

            // Update 컬럼과 값이 셋팅 되지 않은 경우..
            if (this._htParams == null)
            {
                this._Exception = new Exception("No Item");
                return 0;
            }

            this._Exception = null;

            try
            {
                IDictionaryEnumerator enumInterface = this._htParams.GetEnumerator();

                bool first = true;
                // Update 쿼리 구문 만들기
                while (enumInterface.MoveNext())
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        s1.Append(", ");
                    }
                    s1.Append(string.Format("{0}=@{1}", enumInterface.Key.ToString(), enumInterface.Key.ToString()));
                }

                string[] data = new string[4];

                data[0] = TBL;
                data[1] = s1.ToString();
                data[2] = KeyName;
                data[3] = KeyValue;

                query = string.Format("UPDATE {0} SET {1} WHERE {2} = {3}", data);

                comm = new SqlCommand(query, this._conn);

                // Parameter 추가
                enumInterface = this._htParams.GetEnumerator();
                while (enumInterface.MoveNext())
                {
                    comm.Parameters.Add(new SqlParameter(string.Format("@{0}", enumInterface.Key.ToString()), enumInterface.Value.ToString()));
                }

                if (this._conn.State != ConnectionState.Open) this._conn.Open();

                UpdatedRow = comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                this._Exception = ex;
            }
            finally
            {
                comm.Dispose();
            }

            return UpdatedRow;
        }

        /// <summary>
        /// 대상 테이블에서 선택된 컬럼을 삭제 합니다.
        /// </summary>
        /// <param name="TBL">대상 테이블명</param>
        /// <param name="KeyName">키 명</param>
        /// <param name="KeyValue">키 값</param>
        /// <returns>영향 받은 행의 수</returns>
        public int Delete(string TBL, string KeyName, string KeyValue)
        {
            string query = "";
            int DeletedRow = 0;
            SqlCommand comm = null;

            this._Exception = null;

            query = string.Format("DELETE FROM {0} WHERE {1} = @{2}", TBL, KeyName, KeyName);

            comm = new SqlCommand(query, this._conn);

            comm.Parameters.Add(new SqlParameter(string.Format("@{0}", KeyName), KeyValue));

            if (this._conn.State != ConnectionState.Open) this._conn.Open();

            try
            {
                DeletedRow = comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                this._Exception = ex;
            }
            finally
            {
                comm.Dispose();
            }

            return DeletedRow;
        }

        /// <summary>
        /// 대상 테이블로 부터 SELECT 쿼리를 실행합니다.
        /// </summary>
        /// <param name="TBL">대상 테이블명</param>
        /// <returns>SELECT 결과</returns>
        public DataSet Select(string TBL)
        {
            return this.Select(TBL, "*", "", "");
        }

        /// <summary>
        /// 대상 테이블로 부터 SELECT 쿼리를 실행합니다.
        /// </summary>
        /// <param name="TBL">대상 테이블명</param>
        /// <param name="Columns">컬럼명 - 컬럼명은 쉼표(,)로 구분한 MS-SQL 구문입니다.</param>
        /// <returns>SELECT 결과</returns>
        public DataSet Select(string TBL, string Columns)
        {
            return this.Select(TBL, Columns, "", "");
        }

        /// <summary>
        /// 대상 테이블로 부터 SELECT 쿼리를 실행합니다.
        /// </summary>
        /// <param name="TBL">대상 테이블명</param>
        /// <param name="Columns">컬럼명 - 컬럼명은 쉼표(,)로 구분한 MS-SQL 구문입니다.</param>
        /// <param name="WhereCondition">조건절 - WHERE 키워드 없이 나열한 MS-SQL 구문입니다.</param>
        /// <returns>SELECT 결과</returns>
        public DataSet Select(string TBL, string Columns, string WhereCondition)
        {
            return this.Select(TBL, Columns, WhereCondition, "");
        }

        /// <summary>
        /// 대상 테이블로 부터 SELECT 쿼리를 실행합니다.(단, GROUP BY 조건은 사용할 수 없으며, 이 경우는 ExecuteQuery()함수를 이용해 주십시오.)
        /// </summary>
        /// <param name="TBL">대상 테이블명</param>
        /// <param name="Columns">컬럼명 - 컬럼명은 쉼표(,)로 구분한 MS-SQL 구문입니다.</param>
        /// <param name="WhereCondition">조건절 - WHERE 키워드 없이 나열한 MS-SQL 구문입니다.</param>
        /// <param name="OrderCondition">정렬조건 - ORDER BY 키워드 없이 나열한 MS-SQL 구문입니다.</param>
        /// <returns>SELECT 결과</returns>
        public DataSet Select(string TBL, string Columns, string WhereCondition, string OrderCondition)
        {
            SqlDataAdapter adapter = null;
            SqlCommand comm = null;
            DataSet ds = new DataSet();
            string query = "";

            this._Exception = null;

            string[] data = new string[4];
            data[0] = Columns;  // 컬럼명
            data[1] = TBL;      // 대상 테이블명


            // Where 조건
            if (WhereCondition != "" && WhereCondition != "" && WhereCondition.Length > 0)
                data[2] = string.Format("WHERE {0}", WhereCondition);
            else
                data[2] = "";

            // Order 조건
            if (OrderCondition != "" && OrderCondition != "" && OrderCondition.Length > 0)
                data[3] = string.Format("ORDER BY {0}", OrderCondition);
            else
                data[3] = "";


            query = string.Format("SELECT {0} FROM {1} {2} {3}", data);

            comm = new SqlCommand(query, this._conn);

            if (this._conn.State != ConnectionState.Open) this._conn.Open();

            try
            {
                adapter = new SqlDataAdapter(comm);
                adapter.Fill(ds);
            }
            catch (Exception ex)
            {
                this._Exception = ex;
            }
            finally
            {
                comm.Dispose();
            }

            return ds;
        }

        /// <summary>
        /// 지정된 쿼리문을 실행하고 그 결과를 DataSet 으로 반환합니다.
        /// </summary>
        /// <param name="query">실행할 MS-SQL 쿼리 구문입니다.</param>
        /// <returns>쿼리 결과 DataSet</returns>
        public DataSet ExecuteQuery(string query)
        {
            SqlDataAdapter adapter = null;
            SqlCommand comm = null;
            DataSet ds = new DataSet();

            if (this._conn.State != ConnectionState.Open) this._conn.Open();

            comm = new SqlCommand(query, this._conn);

            try
            {
                adapter = new SqlDataAdapter(comm);
                adapter.Fill(ds);
            }
            catch (Exception ex)
            {
                this._Exception = ex;
            }
            finally
            {
                comm.Dispose();
            }

            return ds;
        }

        /// <summary>
        /// 지정된 저장프로시저를 실행합니다.
        /// </summary>
        /// <param name="SPName">저장 프로시저명</param>
        /// <returns>실행결과 DataSet</returns>
        public DataSet ExecuteProcedure(string SPName)
        {
            return ExecuteProcedure(SPName, null);
        }

        /// <summary>
        /// 지정된 저장프로시저를 실행합니다.
        /// </summary>
        /// <param name="SPName">저장 프로시저명</param>
        /// <param name="objSQLParams">SQL Parameters</param>
        /// <returns>실행결과 DataSet</returns>
        public DataSet ExecuteProcedure(string SPName, SqlParameter[] objSQLParams)
        {
            SqlDataAdapter adapter = null;
            SqlCommand comm = null;
            DataSet ds = new DataSet();

            comm = new SqlCommand();

            comm.CommandType = CommandType.StoredProcedure;
            comm.CommandText = SPName;
            comm.Connection = this._conn;

            try
            {
                if (objSQLParams != null)
                {
                    if (objSQLParams.Length > 0)
                    {
                        foreach (SqlParameter param in objSQLParams)
                        {
                            comm.Parameters.Add(param);
                        }
                    }
                }

                adapter = new SqlDataAdapter(comm);
                adapter.Fill(ds);
            }
            catch (Exception ex)
            {
                this._Exception = ex;
            }
            finally
            {
                comm.Dispose();
            }

            return ds;
        }

        /// <summary>   
        /// 두개의 DataTable을 비교해서 다른행을 DataTable형태로 리턴한다.   
        /// </summary>   
        /// <param name="FirstDataTable">FirstDataTable</param>   
        /// <param name="SecondDataTable">SecondDataTable</param>   
        /// <returns>DifferentRecords</returns>   
        public static DataTable GetDifferentRecords(DataTable FirstDataTable, DataTable SecondDataTable)
        {
            //Create Empty Table   
            DataTable ResultDataTable = new DataTable("ResultDataTable");

            using (DataSet ds = new DataSet())
            {
                //Add tables   
                ds.Tables.AddRange(new DataTable[] { FirstDataTable.Copy(), SecondDataTable.Copy() });

                //Get Columns for DataRelation   
                DataColumn[] firstColumns = new DataColumn[ds.Tables[0].Columns.Count];
                for (int i = 0; i < firstColumns.Length; i++)
                {
                    firstColumns[i] = ds.Tables[0].Columns[i];
                }

                DataColumn[] secondColumns = new DataColumn[ds.Tables[1].Columns.Count];
                for (int i = 0; i < secondColumns.Length; i++)
                {
                    secondColumns[i] = ds.Tables[1].Columns[i];
                }

                //Create DataRelation   
                DataRelation r1 = new DataRelation(string.Empty, firstColumns, secondColumns, false);
                ds.Relations.Add(r1);

                DataRelation r2 = new DataRelation(string.Empty, secondColumns, firstColumns, false);
                ds.Relations.Add(r2);

                //Create columns for return table   
                for (int i = 0; i < FirstDataTable.Columns.Count; i++)
                {
                    ResultDataTable.Columns.Add(FirstDataTable.Columns[i].ColumnName, FirstDataTable.Columns[i].DataType);
                }

                ResultDataTable.Columns.Add("Location");

                //첫번째 테이블에만 있고 두번째 테이블에 없다면...
                ResultDataTable.BeginLoadData();
                foreach (DataRow parentrow in ds.Tables[0].Rows)
                {
                    DataRow[] childrows = parentrow.GetChildRows(r1);
                    if (childrows == null || childrows.Length == 0)
                    {
                        ResultDataTable.LoadDataRow(parentrow.ItemArray, true);
                        ResultDataTable.Rows[ResultDataTable.Rows.Count - 1][ResultDataTable.Columns.Count - 1] = DiffLocation.First;
                    }

                }

                //두번째 테이블에만 있고 첫번째 테이블에 없다면...
                foreach (DataRow parentrow in ds.Tables[1].Rows)
                {
                    DataRow[] childrows = parentrow.GetChildRows(r2);
                    if (childrows == null || childrows.Length == 0)
                    {
                        ResultDataTable.LoadDataRow(parentrow.ItemArray, true);
                        ResultDataTable.Rows[ResultDataTable.Rows.Count - 1][ResultDataTable.Columns.Count - 1] = DiffLocation.Second;
                    }
                }
                
                ResultDataTable.EndLoadData();
            }

            return ResultDataTable;
        }

        /// <summary>
        /// Dispose Resource
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (this._conn != null)
                {
                    if (this._conn.State == ConnectionState.Open)
                    {
                        this._conn.Close();
                        this._conn.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                this._Exception = ex;
                this._conn = null;
            }
            System.GC.SuppressFinalize(this);
        }
        #endregion

        #region IDisposable 멤버

        void IDisposable.Dispose()
        {
            this.Dispose();
        }
        #endregion
    }
}

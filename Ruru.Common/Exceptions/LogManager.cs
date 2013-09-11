namespace Ruru.Common.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Reflection;
    using System.Text;
    using System.Transactions;
    using System.Web;
    using Ruru.Common.Config;

    public enum LogType
    {
        Undefined = 0,
        Information,
        Warning,
        Error,
        Performance,
        Debug
    }

    public enum LogSourceType
    {
        Undefined = 0,
        WebPage,
        SharePoint,
        WebService,
        WindowService,
        Console,
        ClassLibrary,
        WebControl
    }

    public static class LogManager
    {
        static string _connectionString;
        static bool _enableLog;
        static bool _enableInformationLog;
        static bool _enableWarningLog;
        static bool _enableDebugLog;
        static bool _enablePerformanceLog;
        static long _performanceMilliseconds;

        static string CONFIG_SECTION = "SEC.VPD.Common";
        static string CONFIG_CATEGORY = "Diagnostics";

        static LogManager()
        {
            try
            {
                // 프로퍼티 설정
                _connectionString = ConfigReader.GetString(CONFIG_SECTION, "ConnectionStrings", "VPD");

                // DB 설정값이 로드되었을 때만 LogManager 사용 가능
                if (!string.IsNullOrEmpty(_connectionString))
                {
                    _enableLog = ConfigReader.GetBoolean(CONFIG_SECTION, CONFIG_CATEGORY, "EnableLog");

                    if (_enableLog)
                    {
                        _enableInformationLog = ConfigReader.GetBoolean(CONFIG_SECTION, CONFIG_CATEGORY, "EnableInformationLog");
                        _enableWarningLog = ConfigReader.GetBoolean(CONFIG_SECTION, CONFIG_CATEGORY, "EnableWarningLog");
                        _enableDebugLog = ConfigReader.GetBoolean(CONFIG_SECTION, CONFIG_CATEGORY, "EnableDebugLog");
                        _enablePerformanceLog = ConfigReader.GetBoolean(CONFIG_SECTION, CONFIG_CATEGORY, "EnablePerformanceLog");

                        if (_enablePerformanceLog)
                        {
                            _performanceMilliseconds = ConfigReader.GetLong(CONFIG_SECTION, CONFIG_CATEGORY, "PerformanceMilliseconds");
                        }
                    }
                }
            }
            catch { }   // 로깅 시 예외 발생은 무조건 스킵
        }

        static bool CalledBySelf()
        {
            // true: LogManager를 원본 소스로 인식하여, 로깅을 중단한다.
            bool returnValue = true;

            // LogManager에서 호출한 공통 모듈에서 오류가 발생해서 다시 LogManager로 돌아온 경우에는 로깅을 스킵한다.
            try
            {
                // 호출 스택에서 역순으로 찾는다.
                // 1. LogManager 클래스가 포함된 프레임은 일단 제외한다.
                // 2. LogManager 클래스가 아닌 클래스가 나온 이후에 다시 LogManager 클래스가 나온다면 로깅 스킵.

                StackTrace st = new StackTrace();

                Type targetType = null;
                Type currentType = st.GetFrame(0).GetMethod().DeclaringType;

                // 0번 인덱스는 현재 메소드이다.
                int i = 1;

                // 처음 프레임부터 다른 클래스가 나올 때까지 일단 스킵 : 내부에서 메소드 호출 시의 처리
                for (; i < st.FrameCount; i++)
                {
                    targetType = st.GetFrame(i).GetMethod().DeclaringType;

                    // 1. LogManager 클래스가 포함된 프레임은 일단 제외한다.
                    if (currentType.Equals(targetType)) continue;

                    // 다른 클래스가 나왔다면 일단 중지
                    break;
                }

                // 다른 네임 스페이스가 나온 이후부터 체크
                for (; i < st.FrameCount; i++)
                {
                    targetType = st.GetFrame(i).GetMethod().DeclaringType;

                    // 2. LogManager 클래스가 아닌 클래스가 나온 이후에 다시 LogManager 클래스가 나온다면 로깅 스킵.
                    if (currentType.Equals(targetType)) return true;

                    // 다른 클래스일 경우, LogManager에서 호출한 내용이 아님.
                    returnValue = false;
                }

                // 여기서 false로 설정하지 않음. 첫번째 루프에서 끝까지 간 경우, 반환값은 true여야 함.
            }
            catch { }   // 로깅 시 예외 발생은 무조건 스킵

            return returnValue;
        }

        #region 로그 등록

        #region WriteInformation

        /// <summary>
        /// 메시지 로깅.
        /// </summary>
        /// <param name="sourceType">Source 타입</param>
        /// <param name="message">메시지</param>
        /// <param name="message">상세 정보. 없을 경우, null.</param>
        public static void WriteInformation(LogSourceType sourceType, string message, string description)
        {
            // 로깅 사용하지 않을 경우 종료
            if (!_enableLog) return;
            if (!_enableInformationLog) return;

            // StackTrace 원본이 LogManager일 경우 종료
            if (CalledBySelf()) return;

            try
            {
                // 필드값 생성
                List<SqlParameter> values = CreateDefaultParameters(sourceType, LogType.Information, message, description);

                // DB에 쓰기
                InsertIntoDatabase(values);
            }
            catch { }   // 로깅 시 예외 발생은 무조건 스킵
        }

        /// <summary>
        /// 메시지 로깅.
        /// </summary>
        /// <param name="sourceType">Source 타입</param>
        /// <param name="data">LogData 개체. null이 아닐 경우, 포함된 DataStack 정보를 기록함.</param>
        public static void WriteInformation(LogSourceType sourceType, LogData data)
        {
            // 로깅 사용하지 않을 경우 종료
            if (!_enableLog) return;
            if (!_enableInformationLog) return;

            // StackTrace 원본이 LogManager일 경우 종료
            if (CalledBySelf()) return;

            try
            {
                // 필드값 생성
                List<SqlParameter> values = CreateDefaultParameters(sourceType, LogType.Information, data);

                // DB에 쓰기
                InsertIntoDatabase(values);
            }
            catch { }   // 로깅 시 예외 발생은 무조건 스킵
        }

        #endregion

        #region WriteWarning

        /// <summary>
        /// 경고 메시지 로깅.
        /// </summary>
        /// <param name="sourceType">Source 타입</param>
        /// <param name="message">메시지</param>
        /// <param name="description">상세 정보. 없을 경우, null.</param>
        public static void WriteWarning(LogSourceType sourceType, string message, string description)
        {
            // 로깅 사용하지 않을 경우 종료
            if (!_enableLog) return;
            if (!_enableWarningLog) return;

            // StackTrace 원본이 LogManager일 경우 종료
            if (CalledBySelf()) return;

            try
            {
                // 필드값 생성
                List<SqlParameter> values = CreateDefaultParameters(sourceType, LogType.Warning, message, description);

                // DB에 쓰기
                InsertIntoDatabase(values);
            }
            catch { }   // 로깅 시 예외 발생은 무조건 스킵
        }

        /// <summary>
        /// 경고 메시지 로깅.
        /// </summary>
        /// <param name="sourceType">Source 타입</param>
        /// <param name="data">LogData 개체. null이 아닐 경우, 포함된 DataStack 정보를 기록함.</param>
        public static void WriteWarning(LogSourceType sourceType, LogData data)
        {
            // 로깅 사용하지 않을 경우 종료
            if (!_enableLog) return;
            if (!_enableWarningLog) return;

            // StackTrace 원본이 LogManager일 경우 종료
            if (CalledBySelf()) return;

            try
            {
                // 필드값 생성
                List<SqlParameter> values = CreateDefaultParameters(sourceType, LogType.Warning, data);

                // DB에 쓰기
                InsertIntoDatabase(values);
            }
            catch { }   // 로깅 시 예외 발생은 무조건 스킵
        }

        #endregion

        #region WriteError

        /// <summary>
        /// Exception.Data에 데이터 추가. 예외 처리됨.
        /// 이미 키가 있을 경우, 덮어써짐.
        /// </summary>
        /// <param name="ex">Exception 개체</param>
        /// <param name="key">키</param>
        /// <param name="value">값</param>
        public static void AddExceptionData(ref Exception ex, string key, object value)
        {
            if (ex == null) return;
            if (string.IsNullOrEmpty(key)) return;
            try
            {
                // 기존 값 삭제
                if (ex.Data.Contains(key)) ex.Data.Remove(key);

                // 값 추가
                ex.Data.Add(key, value);
            }
            catch { }   // 로깅 시 예외 발생은 무조건 스킵
        }

        /// <summary>
        /// Exception.Data에 LogData에 포함된 Values 데이터 추가. 예외 처리됨.
        /// 이미 키가 있을 경우, 덮어써짐.
        /// </summary>
        /// <param name="ex">Exception 개체</param>
        /// <param name="data">Value가 포함된 LogData 개체</param>
        public static void AddExceptionData(ref Exception ex, LogData data)
        {
            if (ex == null) return;
            if (data == null) return;
            if (data.Values == null || data.Values.Count == 0) return;
            try
            {
                foreach (string key in data.Values.Keys)
                {
                    AddExceptionData(ref ex, key, data.Values[key]);
                }
            }
            catch { }   // 로깅 시 예외 발생은 무조건 스킵
        }

        /// <summary>
        /// 오류 로깅.
        /// </summary>
        /// <param name="sourceType">Source 타입</param>
        /// <param name="ex">Exception 개체</param>
        public static void WriteError(LogSourceType sourceType, Exception ex)
        {
            // 로깅 사용하지 않을 경우 종료
            if (!_enableLog) return;

            // StackTrace 원본이 LogManager일 경우 종료
            if (CalledBySelf()) return;

            try
            {
                string message = ex.Message;

                StringBuilder description = new StringBuilder(1024);
                GatherExceptionDescription(ref description, ex, 0);

                // 필드값 생성
                List<SqlParameter> values = CreateDefaultParameters(sourceType, LogType.Error, message, description.ToString());

                // DB에 쓰기
                InsertIntoDatabase(values);
            }
            catch { }   // 로깅 시 예외 발생은 무조건 스킵
        }

        private static void GatherExceptionDescription(ref StringBuilder buffer, Exception ex, int level)
        {
            try
            {
                // Inner Exception Header
                if (level > 0)
                {
                    buffer.AppendLine("");
                    buffer.AppendLine(string.Format("[Inner Exception #{0}]", level));
                }

                // Exception Information
                buffer.AppendLine(string.Format("{0}: {1}", ex.GetType().FullName, ex.Message));
                buffer.AppendLine(ex.StackTrace);

                // Exception Data
                if (ex.Data != null && ex.Data.Count > 0)
                {
                    foreach (string key in ex.Data.Keys)
                    {
                        object data = ex.Data[key];
                        if (data == null)
                        {
                            data = "{null}";
                        }
                        else if (data == DBNull.Value)
                        {
                            data = "{DBNull}";
                        }
                        buffer.AppendLine(string.Format("> {0} = [{1}]", key, data));
                    }
                }

                // Inner Exception
                if (ex.InnerException != null)
                {
                    GatherExceptionDescription(ref buffer, ex.InnerException, level + 1);
                }
            }
            catch { }   // 로깅 시 예외 발생은 무조건 스킵
        }

        #endregion

        #region WritePerformance

        /// <summary>
        /// 성능 로깅.
        /// </summary>
        /// <param name="sourceType">Source 타입</param>
        /// <param name="data">LogData 개체. null일 경우, 로그 남지 않음.</param>
        public static void WritePerformance(LogSourceType sourceType, LogData data)
        {
            // 로깅 사용하지 않을 경우 종료
            if (!_enableLog) return;
            if (!_enablePerformanceLog) return;

            // StackTrace 원본이 LogManager일 경우 종료
            if (CalledBySelf()) return;

            try
            {
                // 소요 시간이 컨피그에 정의된 시간보다 같거나 클 경우에만 기록
                // ->  컨피그 정의 시간이 0 이하이면 항상 기록됨
                if (data != null)
                {
                    // 타이머 종료
                    data.StopTimer();

                    if (data.ElapsedMilliseconds >= _performanceMilliseconds)
                    {
                        string message = data.Description;
                        string description = data.ValueStack;

                        // 필드값 생성
                        List<SqlParameter> values = CreateDefaultParameters(sourceType, LogType.Performance, message, description);
                        values.Add(new SqlParameter("@StartTime", data.StartTime.ToString("yyyy-MM-dd HH:mm:ss.fff")));
                        values.Add(new SqlParameter("@EndTime", data.EndTime.ToString("yyyy-MM-dd HH:mm:ss.fff")));
                        values.Add(new SqlParameter("@ElapsedMilliseconds", data.ElapsedMilliseconds));

                        // DB에 쓰기
                        InsertIntoDatabase(values);
                    }
                }
            }
            catch { }   // 로깅 시 예외 발생은 무조건 스킵
        }

        #endregion

        #region WriteDebug

        /// <summary>
        /// 디버그 로깅.
        /// </summary>
        /// <param name="sourceType">Source 타입</param>
        /// <param name="message">메시지</param>
        /// <param name="description">상세 정보. 없을 경우, null.</param>
        public static void WriteDebug(LogSourceType sourceType, string message, string description)
        {
            // 로깅 사용하지 않을 경우 종료
            if (!_enableLog) return;
            if (!_enableDebugLog) return;

            // StackTrace 원본이 LogManager일 경우 종료
            if (CalledBySelf()) return;

            try
            {
                // 필드값 생성
                List<SqlParameter> values = CreateDefaultParameters(sourceType, LogType.Debug, message, description);

                // DB에 쓰기
                InsertIntoDatabase(values);
            }
            catch { }   // 로깅 시 예외 발생은 무조건 스킵
        }

        /// <summary>
        /// 디버그 로깅.
        /// </summary>
        /// <param name="sourceType">Source 타입</param>
        /// <param name="data">LogData 개체. null이 아닐 경우, 포함된 DataStack 정보를 기록함.</param>
        public static void WriteDebug(LogSourceType sourceType, LogData data)
        {
            // 로깅 사용하지 않을 경우 종료
            if (!_enableLog) return;
            if (!_enableDebugLog) return;

            // StackTrace 원본이 LogManager일 경우 종료
            if (CalledBySelf()) return;

            try
            {
                // 필드값 생성
                List<SqlParameter> values = CreateDefaultParameters(sourceType, LogType.Debug, data);

                // DB에 쓰기
                InsertIntoDatabase(values);
            }
            catch { }   // 로깅 시 예외 발생은 무조건 스킵
        }

        #endregion

        #region WriteLog

        /// <summary>
        /// 로깅. 타입 설정 가능.
        /// </summary>
        /// <param name="sourceType">Source 타입</param>
        /// <param name="logType">로그 타입.</param>
        /// <param name="message">메시지.</param>
        /// <param name="description">상세 정보.</param>
        public static void WriteLog(LogSourceType sourceType, LogType logType, string message, string description)
        {
            // 로깅 사용하지 않을 경우 종료
            if (!_enableLog) return;

            // StackTrace 원본이 LogManager일 경우 종료
            if (CalledBySelf()) return;

            // 로깅 타입에 따라 로깅 사용 여부 확인
            switch (logType)
            {
                case LogType.Debug:
                    if (!_enableDebugLog) return;
                    break;
                case LogType.Information:
                    if (!_enableInformationLog) return;
                    break;
                case LogType.Performance:
                    if (!_enablePerformanceLog) return;
                    break;
                case LogType.Warning:
                    if (!_enableWarningLog) return;
                    break;
            }

            try
            {
                // 필드값 생성
                List<SqlParameter> values = CreateDefaultParameters(sourceType, logType, message, description);

                // DB에 쓰기
                InsertIntoDatabase(values);
            }
            catch { }   // 로깅 시 예외 발생은 무조건 스킵
        }

        #endregion

        #endregion

        #region DB 필드값 생성

        private static List<SqlParameter> CreateDefaultParameters(LogSourceType sourceType, LogType logType, LogData data)
        {
            // ProcessInfo에서 추출
            string message = null;
            string description = null;
            if (data != null)
            {
                message = data.Description;
                description = data.ValueStack;
            }

            // 필드값 생성
            return CreateDefaultParameters(sourceType, logType, message, description);
        }

        private static List<SqlParameter> CreateDefaultParameters(LogSourceType sourceType, LogType logType, string message, string detail)
        {
            List<SqlParameter> values = new List<SqlParameter>();

            // 로그 타입
            try
            {
                values.Add(new SqlParameter("@LogType", logType.ToString().Substring(0, 1)));
            }
            catch { }   // 로깅 시 예외 발생은 무조건 스킵

            // 로그 소스 타입
            try
            {
                values.Add(new SqlParameter("@LogSourceType", sourceType.ToString()));
            }
            catch { }   // 로깅 시 예외 발생은 무조건 스킵

            // 요청자 정보 : 자동
            try
            {
                // 웹 요청인 경우, 요청자 정보 자동 설정
                if (HttpContext.Current != null)
                {
                    values.Add(new SqlParameter("@RequestIP", HttpContext.Current.Request.UserHostAddress));
                    values.Add(new SqlParameter("@RequestUser", HttpContext.Current.User.Identity.Name));
                    values.Add(new SqlParameter("@RequestUrl", HttpContext.Current.Request.Url.ToString()));

                    //VpdUserInfo userInfo = VpdUserInfo.GetCurrentUserInfo();
                    //if (userInfo != null)
                    //{
                    //    values.Add(new SqlParameter("@RequestUserName", userInfo.UserName));
                    //}
                }
            }
            catch { }   // 로깅 시 예외 발생은 무조건 스킵

            // 어셈블리 정보 : 자동
            try
            {
                // 호출 메소드 정보 찾기
                MethodBase callerMethod = null;
                Type callerType = null;

                // 호출 스택에서 역순으로 찾는다.
                // - 현재 네임 스페이스(첫번째 호출 스택)와 동일한 네임 스페이스에 포함된 프레임은 제외한다.

                StackTrace st = new StackTrace();
                string currentNamespace = st.GetFrame(0).GetMethod().DeclaringType.Namespace;

                for (int i = 1; i < st.FrameCount; i++)
                {
                    callerMethod = st.GetFrame(i).GetMethod();
                    callerType = callerMethod.DeclaringType;

                    // 현재 네임 스페이스와 동일한 네임 스페이스에 포함된 프레임은 제외한다.
                    if (callerMethod.DeclaringType.Namespace == currentNamespace) continue;

                    // 가장 최하위 호출 메소드 찾음
                    break;
                }

                // 호출 모듈 정보
                values.Add(new SqlParameter("@AssemblyName", callerType.Assembly.FullName));
                values.Add(new SqlParameter("@Namespace", callerType.Namespace));
                values.Add(new SqlParameter("@ClassName", callerType.Name));
                values.Add(new SqlParameter("@MethodName", callerMethod.Name));

                // * message가 생략된 경우, message에 '클래스명.메소드명'을 넣는다.
                if (string.IsNullOrEmpty(message))
                {
                    message = string.Format("{0}.{1}", callerType.Name, callerMethod.Name);
                }
            }
            catch { }   // 로깅 시 예외 발생은 무조건 스킵

            // 수행 머신 정보 : 자동
            try
            {
                values.Add(new SqlParameter("@ServerMachine", System.Environment.MachineName));
            }
            catch { }   // 로깅 시 예외 발생은 무조건 스킵

            // 로그 메시지
            values.Add(new SqlParameter("@LogMessage", message));
            values.Add(new SqlParameter("@LogDetail", detail));

            return values;
        }

        #endregion

        #region DB 인서트

        private static void InsertIntoDatabase(List<SqlParameter> values)
        {
            try
            {
                // 별도의 DAC단 클래스를 만들지 않음. 직접 SP 호출.
                // 새 트랜잭션으로 실행: 이 범위에서 발생하는 오류는 상위 트랜잭션을 Rollback하지 않도록 함
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    using (SqlConnection con = new SqlConnection(_connectionString))
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "Stp_Common_Logs_Ins";
                            cmd.Parameters.AddRange(values.ToArray());

                            // 실행
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // 완료
                    tx.Complete();
                }
            }
            catch { }   // 로깅 시 예외 발생은 무조건 스킵
        }

        #endregion
    }
}

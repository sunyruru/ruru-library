namespace Ruru.Common.Globalization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Resources;
    using System.Text;
    using System.Web;
    using Ruru.Common.Exceptions;

    public static class ResourceReader
    {
        /// <summary>
        /// resource 리소스 파일에서 key 값을 읽어온다.
        /// 현재 Thread의 UICulture 값을 기준으로 한다.
        /// 리소스 파일에서 키를 찾지 못할 경우, null이 반환된다.
        /// </summary>
        /// <param name="resource">대상 리소스 파일명</param>
        /// <param name="key">리소스 파일에 포함되어 있는 키</param>
        /// <returns>리소스 파일에서 키를 찾지 못할 경우, null이 반환된다.</returns>
        public static object GetValue(string resource, string key)
        {
            return GetValue(resource, key, null);
        }

        /// <summary>
        /// resource 리소스 파일에서 key 값을 읽어온다.
        /// 리소스 파일에서 키를 찾지 못할 경우, null이 반환된다.
        /// </summary>
        /// <param name="resource">대상 리소스 파일명</param>
        /// <param name="key">리소스 파일에 포함되어 있는 키</param>
        /// <param name="cultureInfo">UICulture</param>
        /// <returns>리소스 파일에서 키를 찾지 못할 경우, null이 반환된다.</returns>
        public static object GetValue(string resource, string key, CultureInfo cultureInfo)
        {
            try
            {
                // 기본 컬쳐는 현재 스레드의 컬쳐를 기준으로 함
                if (cultureInfo == null) cultureInfo = UICultureInfo.Current;

                // HttpContext.GetGlobalResourceObject에서 해당 키가 존재하지 않을 경우, 아무 예외도 발생하지 않고 null 반환됨.
                object value = HttpContext.GetGlobalResourceObject(resource, key, cultureInfo);

                // 지정된 키가 없을 경우 null임
                if (value == null)
                {
                    // 로그를 남기기 위해 예외 발생
                    throw new Exception(string.Format("{0} 리소스에 {1} 키가 없습니다.", resource, key));
                }

                // 가져온 값 반환
                return value;
            }
            catch (Exception ex)
            {
                // 로그만 남기고 예외는 무시
                LogManager.WriteError(LogSourceType.ClassLibrary, ex);

                // null 반환
                return null;
            }
        }

        /// <summary>
        /// resource 리소스 파일에서 key 값을 읽어온다.
        /// 현재 Thread의 UICulture 값을 기준으로 한다.
        /// 리소스 파일에서 키를 찾지 못할 경우, '리소스명:키' 형식으로 반환된다.
        /// </summary>
        /// <param name="resource">대상 리소스 파일명</param>
        /// <param name="key">리소스 파일에 포함되어 있는 키</param>
        /// <returns>리소스 파일에서 키를 찾지 못할 경우, '리소스명:키' 형식으로 반환된다.</returns>
        public static string GetString(string resource, string key)
        {
            return GetString(resource, key, null);
        }

        /// <summary>
        /// resource 리소스 파일에서 key 값을 읽어온다.
        /// 리소스 파일에서 키를 찾지 못할 경우, '리소스명:키' 형식으로 반환된다.
        /// </summary>
        /// <param name="resource">대상 리소스 파일명</param>
        /// <param name="key">리소스 파일에 포함되어 있는 키</param>
        /// <param name="cultureInfo">UICulture</param>
        /// <returns>리소스 파일에서 키를 찾지 못할 경우, '리소스명:키' 형식으로 반환된다.</returns>
        public static string GetString(string resource, string key, CultureInfo cultureInfo)
        {
            // 키값을 가져옴
            object value = GetValue(resource, key, cultureInfo);

            // 키값이 없으면 null이 반환됨.
            if (value == null)
            {
                // '리소스명:키' 형식으로 반환.
                return string.Format("{0}:{1}", resource, key);
            }
            else
            {
                return value.ToString();
            }
        }
    }
}

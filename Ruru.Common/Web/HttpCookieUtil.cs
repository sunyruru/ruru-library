namespace Ruru.Common.Web
{
    using System;

    /// <summary>
    /// 브라우저 쿠키 제어
    /// </summary>
    public static class HttpCookieUtil
    {

        //public static DateTime BrowserCookie = DateTime.MinValue;
        /// <summary>
        /// 브라우저 쿠키 Expire
        /// </summary>
        public static readonly DateTime BrowserCookie = DateTime.MinValue;

        #region Save

        /// <summary>
        /// 브라우저 쿠키 저장
        /// </summary>
        /// <param name="cookieName">쿠키 이름</param>
        /// <param name="value">쿠키 값. Base64 인코딩하여 저장된다.</param>
        public static void Save(string cookieName, string value)
        {
            Save(null, cookieName, value, BrowserCookie);
        }

        /// <summary>
        /// 쿠키 저장
        /// </summary>
        /// <param name="cookieName">쿠키 이름</param>
        /// <param name="value">쿠키 값. Base64 인코딩하여 저장된다.</param>
        /// <param name="expirationMinutes">만료 시간(분). 현재 시각으로부터 경과 시간. 0이면 브라우저 쿠키로 저장. 기본값 0.</param>
        public static void Save(string cookieName, string value, int expirationMinutes)
        {
            Save(null, cookieName, value, expirationMinutes);
        }

        /// <summary>
        /// 쿠키 저장
        /// </summary>
        /// <param name="cookieName">쿠키 이름</param>
        /// <param name="value">쿠키 값. Base64 인코딩하여 저장된다.</param>
        /// <param name="expires">만료 일시. CookieUtil.BrowserCookie 값을 넘기면 브라우저 쿠키로 저장. 기본값 CookieUtil.BrowserCookie.</param>
        public static void Save(string cookieName, string value, DateTime expires)
        {
            Save(null, cookieName, value, expires);
        }

        /// <summary>
        /// 브라우저 쿠키 저장
        /// </summary>
        /// <param name="response">HttpResponse 개체. null이면 HttpContext.Current.Response 사용.</param>
        /// <param name="cookieName">쿠키 이름</param>
        /// <param name="value">쿠키 값. Base64 인코딩하여 저장된다.</param>
        public static void Save(System.Web.HttpResponse response, string cookieName, string value)
        {
            Save(response, cookieName, value, BrowserCookie);
        }

        /// <summary>
        /// 쿠키 저장
        /// </summary>
        /// <param name="response">HttpResponse 개체. null이면 HttpContext.Current.Response 사용.</param>
        /// <param name="cookieName">쿠키 이름</param>
        /// <param name="value">쿠키 값. Base64 인코딩하여 저장된다.</param>
        /// <param name="expirationMinutes">만료 시간(분). 현재 시각으로부터 경과 시간. 0이면 브라우저 쿠키로 저장. 기본값 0.</param>
        public static void Save(System.Web.HttpResponse response, string cookieName, string value, int expirationMinutes)
        {
            DateTime expires = BrowserCookie;
            if (expirationMinutes > 0)
            {
                expires = DateTime.Now.AddMinutes(expirationMinutes);
            }
            Save(response, cookieName, value, expires);
        }

        /// <summary>
        /// 쿠키 저장
        /// </summary>
        /// <param name="response">HttpResponse 개체. null이면 HttpContext.Current.Response 사용.</param>
        /// <param name="cookieName">쿠키 이름</param>
        /// <param name="value">쿠키 값. Base64 인코딩하여 저장된다.</param>
        /// <param name="expires">만료 일시. CookieUtil.BrowserCookie 값을 넘기면 브라우저 쿠키로 저장. 기본값 CookieUtil.BrowserCookie.</param>
        public static void Save(System.Web.HttpResponse response, string cookieName, string value, DateTime expires)
        {
            // 쿠키 생성
            System.Web.HttpCookie cookie = new System.Web.HttpCookie(cookieName);

            // 만료 설정
            if (expires == DateTime.MinValue)
            {
                // 브라우저 쿠키
            }
            else
            {
                cookie.Expires = expires;
            }

            // 값 설정: Base64 인코딩 (한글, 특수문자 등 제거)
            if (string.IsNullOrEmpty(value))
            {
                cookie.Value = value;
            }
            else
            {
                byte[] valueBytes = System.Text.Encoding.Default.GetBytes(value);
                cookie.Value = Convert.ToBase64String(valueBytes);
            }

            // 응답 검증
            if (response == null)
            {
                response = System.Web.HttpContext.Current.Response;
            }

            // 일단 제거
            Delete(response, cookieName);

            // 추가
            response.Cookies.Add(cookie);
        }

        #endregion

        #region Read

        /// <summary>
        /// 쿠키에서 값 읽음
        /// </summary>
        /// <param name="cookieName">쿠키 이름</param>
        /// <returns>쿠키 값. 없으면 null</returns>
        public static string Read(string cookieName)
        {
            return Read(null, cookieName);
        }

        /// <summary>
        /// 쿠키에서 값 읽음
        /// </summary>
        /// <param name="request">HttpRequest 개체. null이면 HttpContext.Current.Request 사용.</param>
        /// <param name="cookieName">쿠키 이름</param>
        /// <returns>쿠키 값. 없으면 null</returns>
        public static string Read(System.Web.HttpRequest request, string cookieName)
        {
            string returnValue = null;

            if (request == null)
            {
                request = System.Web.HttpContext.Current.Request;
            }

            System.Web.HttpCookie cookie = request.Cookies[cookieName];
            if (cookie != null)
            {
                // 값 읽음 : Base64 인코딩 되어 있음.
                string value = cookie.Value;

                // 값 처리
                if (string.IsNullOrEmpty(value))
                {
                    returnValue = value;   // string.empty일 경우의 처리
                }
                else
                {
                    byte[] valueBytes = Convert.FromBase64String(value);
                    returnValue = System.Text.Encoding.Default.GetString(valueBytes);
                }
            }
            return returnValue;
        }

        #endregion

        #region Delete

        /// <summary>
        /// 응답 쿠키 제거
        /// </summary>
        /// <param name="cookieName">쿠키 이름</param>
        public static void Delete(string cookieName)
        {
            Delete(null, cookieName);
        }

        /// <summary>
        /// 응답 쿠키 제거
        /// </summary>
        /// <param name="response">HttpResponse 개체. null이면 HttpContext.Current.Response 사용.</param>
        /// <param name="cookieName">쿠키 이름</param>
        public static void Delete(System.Web.HttpResponse response, string cookieName)
        {
            if (response == null)
            {
                response = System.Web.HttpContext.Current.Response;
            }
            response.Cookies.Remove(cookieName);
        }

        #endregion
    }
}

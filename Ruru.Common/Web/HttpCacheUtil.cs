namespace Ruru.Common.Web
{
    using System;

    /// <summary>
    /// 브라우저 쿠키를 제어하는 클래스
    /// </summary>
    public static class HttpCacheUtil
    {
        #region Get

        /// <summary>
        /// 서버 캐시로부터 값을 읽음. object 타입으로 반환. 값이 없으면 null 반환.
        /// </summary>
        /// <param name="key">캐시 키</param>
        /// <returns>object 타입으로 반환. 값이 없으면 null 반환.</returns>
        public static object Get(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;
            return System.Web.HttpContext.Current.Cache[key];
        }

        /// <summary>
        /// 서버 캐시로부터 값을 읽음. 값이 없으면 null 반환. 타입 변환 실패 시 null 반환.
        /// </summary>
        /// <param name="key">캐시 키</param>
        /// <param name="type">변환할 Type</param>
        /// <returns>값이 없으면 null 반환. 타입 변환 실패 시 null 반환.</returns>
        public static object Get(string key, Type type)
        {
            if (object.ReferenceEquals(type, null) == true) return null;

            object value = Get(key);
            if (object.ReferenceEquals(value, null) == true) return null;

            return Convert.ChangeType(value, type);
        }

        #endregion

        #region Add

        /// <summary>
        /// 캐시에 값 저장
        /// </summary>
        /// <param name="key">키. 키가 존재할 경우, 삭제하고 새로 생성.</param>
        /// <param name="value">저장할 값. null이라도 저장됨.</param>
        public static void Add(string key, object value)
        {
            Add(key, value, 30, false);
        }

        /// <summary>
        /// 캐시에 값 저장
        /// </summary>
        /// <param name="key">키. 키가 존재할 경우, 삭제하고 새로 생성.</param>
        /// <param name="value">저장할 값. null이라도 저장됨.</param>
        /// <param name="expirationMinutes">만료 시간 (분). 현재 시각부터의 경과 시간. 0일 경우, 기본 30분.</param>
        public static void Add(string key, object value, int expirationMinutes)
        {
            Add(key, value, expirationMinutes, false);
        }

        /// <summary>
        /// 캐시에 값 저장
        /// </summary>
        /// <param name="key">키. 키가 존재할 경우, 삭제하고 새로 생성.</param>
        /// <param name="value">저장할 값. null이라도 저장됨.</param>
        /// <param name="expirationMinutes">만료 시간 (분). 현재 시각부터의 경과 시간. 0일 경우, 기본 30분.</param>
        /// <param name="sliding">상대 만료 여부. 기본값 false.</param>
        public static void Add(string key, object value, int expirationMinutes, bool sliding)
        {
            // 일단 삭제
            Remove(key);

            // 경과 시간 검증 : 기본값 30분
            if (expirationMinutes < 1) expirationMinutes = 30;

            // 만료 설정
            DateTime absoluteExpiration = System.Web.Caching.Cache.NoAbsoluteExpiration;
            TimeSpan slidingExpiration = System.Web.Caching.Cache.NoSlidingExpiration;
            if (sliding)
            {
                slidingExpiration = new TimeSpan(0, expirationMinutes, 0);
            }
            else
            {
                absoluteExpiration = DateTime.Now.AddMinutes(expirationMinutes);
            }

            // 새로 생성
            System.Web.HttpContext.Current.Cache.Insert(key, value, null, absoluteExpiration, slidingExpiration);
        }

        #endregion

        #region Remove

        /// <summary>
        /// 캐시에서 값 삭제
        /// </summary>
        /// <param name="key">캐시 키</param>
        public static void Remove(string key)
        {
            if (string.IsNullOrEmpty(key)) return;

            // 캐시에서 삭제
            System.Web.HttpContext.Current.Cache.Remove(key);
        }

        #endregion    
    }
}

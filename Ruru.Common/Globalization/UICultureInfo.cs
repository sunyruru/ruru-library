namespace Ruru.Common.Globalization
{
    using System.Collections.Generic;
    using System.Globalization;

    public enum UICultureCodeType
    {
        LCID = 0,
        TwoLetterISO
    }

    public static class UICultureInfo
    {
        static List<int> _enabledUICultures;
        static int _defaultUICulture;

        //$$소스분석에 의해 주석처리됨.$$
        //public static int LCID_KOREAN = 1042;
        public const int LCID_KOREAN = 1042;

        //$$소스분석에 의해 주석처리됨.$$
        //public static int LCID_ENGLISH = 1033;
        public const int LCID_ENGLISH = 1033;

        static UICultureInfo()
        {
            // Config에서 읽음
            string defaultUICultureValue = Ruru.Common.Config.ConfigReader.GetString("SEC.VPD.Common", "Globalization", "DefaultUICulture");
            if (!string.IsNullOrEmpty(defaultUICultureValue))
            {
                int.TryParse(defaultUICultureValue, out _defaultUICulture);
            }
            if (_defaultUICulture == 0) _defaultUICulture = LCID_KOREAN;

            // Config에서 읽음
            _enabledUICultures = new List<int>();
            string enabledUICulturesValue = Ruru.Common.Config.ConfigReader.GetString("SEC.VPD.Common", "Globalization", "EnabledUICultures");
            if (!string.IsNullOrEmpty(enabledUICulturesValue))
            {
                string[] lcids = enabledUICulturesValue.Split(';');
                foreach (string lcid in lcids)
                {
                    int lcidNumber = -1;
                    if (int.TryParse(lcid, out lcidNumber))
                    {
                        if (lcidNumber > 0)
                        {
                            _enabledUICultures.Add(lcidNumber);
                        }
                    }
                }
            }
            if (_enabledUICultures.Count == 0)
            {
                _enabledUICultures.Add(LCID_KOREAN);
            }
        }

        /// <summary>
        /// 현재 UI Culture를 반환함. 만약 지원하지 않는 Culture로 설정되어 있다면 기본 UI Culture를 반환함.
        /// </summary>
        public static CultureInfo Current
        {
            get
            {
                try
                {
                    if (EnabledUICultures.Contains(CultureInfo.CurrentUICulture.LCID))
                    {
                        return CultureInfo.CurrentUICulture;
                    }
                    else
                    {
                        return new CultureInfo(DefaultUICulture);
                    }
                }
                catch
                {
                    return CultureInfo.CurrentUICulture;
                }
            }
        }

        /// <summary>
        /// 스크립트 등의 경로가 언어 설정 별로 나뉘어져 있을 때 적합한 경로를 만들어서 반환해준다.
        /// 경로상에 현재 언어값을 LCID로 설정해준다.
        /// </summary>
        /// <param name="pathFormat">string.Format 메소드의 인수와 동일. 현재 언어값을 넣을 곳을 {0}으로 표시. (예: /_layouts/{0}/core.js)</param>
        /// <returns></returns>
        public static string BuildUICulturePath(string pathFormat)
        {
            return BuildUICulturePath(pathFormat, UICultureCodeType.LCID);
        }

        /// <summary>
        /// 스크립트 등의 경로가 언어 설정 별로 나뉘어져 있을 때 적합한 경로를 만들어서 반환해준다.
        /// </summary>
        /// <param name="pathFormat">string.Format 메소드의 인수와 동일. 현재 언어값을 넣을 곳을 {0}으로 표시. (예: /_layouts/{0}/core.js)</param>
        /// <param name="codeType">현재 언어값을 표시할 방식. LCID와 TwoLetterISOLanguageName 사용 가능. 기본값은 LCID</param>
        /// <returns></returns>
        public static string BuildUICulturePath(string pathFormat, UICultureCodeType codeType)
        {
            if (codeType == UICultureCodeType.TwoLetterISO)
            {
                return string.Format(pathFormat, Current.TwoLetterISOLanguageName);
            }
            else
            {
                return string.Format(pathFormat, Current.LCID);
            }
        }

        /// <summary>
        /// 기본 UI Culture LCID를 반환.
        /// </summary>
        public static int DefaultUICulture
        {
            get { return _defaultUICulture; }
        }

        /// <summary>
        /// 서버에서 지원하는 UI Culture LCID를 반환함.
        /// </summary>
        public static List<int> EnabledUICultures
        {
            get { return _enabledUICultures; }
        }
    }
}

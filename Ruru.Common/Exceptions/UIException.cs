namespace Ruru.Common.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Text;

    public class UIException : System.Exception
    {
        #region 리소스 키 상수

        //$$소스분석에 의해 수정됨.$$
        //public static string UnknownException = "UIException_UnknownException";
        public const string UnknownException = "UIException_UnknownException";

        //$$소스분석에 의해 수정됨.$$
        //public static string UndefinedException = "UIException_UndefinedException";
        public const string UndefinedException = "UIException_UndefinedException";

        #endregion

        #region UIException 개체

        string _resourceKey;
        string _message;

        #region 코드 Analysis 결과 때문에 추가함

        /// <summary>
        /// 이 생성자로 생성하지 마십시오.
        /// </summary>
        public UIException()
            : this(null, null)
        { }

        /// <summary>
        /// 이 생성자로 생성하지 마십시오.
        /// </summary>
        protected UIException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        #endregion

        public UIException(string resourceKey)
            : this(resourceKey, null)
        { }

        public UIException(string resourceKey, Exception innerException)
            : base(resourceKey, innerException)
        {
            if (string.IsNullOrEmpty(resourceKey))
            {
                resourceKey = UIException.UnknownException;
            }

            _resourceKey = resourceKey;

            this.Data.Add("ResourceKey", _resourceKey);
        }

        public override string Message
        {
            get
            {
                if (string.IsNullOrEmpty(_message))
                {
                    _message = Globalization.ResourceReader.GetString("UserMessage", this.ResourceKey);
                }
                return _message;
            }
        }

        public string ResourceKey
        {
            get
            {
                return _resourceKey;
            }
        }

        #endregion
    }
}

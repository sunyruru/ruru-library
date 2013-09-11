namespace Ruru.Common.Web.UI
{
    using System.Collections;
    using System.Collections.Specialized;
    using System.Text;
    using System.Web.UI.WebControls;

    /// <summary>
    /// Context Menu 호출시 Mene Type,Disable 여부,Custom Function 목록,function 파라미터 Value를 소스 컨트롤의 Attritue에 Append 함 
    /// </summary>
    public class ContextMenu
    {
        #region GetAttributeHtml: Attribute HTML 코드 생성

        /// <summary>
        /// Attribute 생성
        /// </summary>
        /// <param name="obj">context menu가 수행될 컨트롤 </param>
        /// <param name="menuType">context menu 형태</param>
        public string GetAttributeHtml(string menuType)
        {
            return GetAttributeHtml(menuType, (Hashtable)null, null, null, SelectionType.Default);
        }

        /// <summary>
        /// Attribute 생성
        /// </summary>
        /// <param name="obj">context menu가 수행될 컨트롤 </param>
        /// <param name="menuType">context menu 형태</param>
        /// <param name="selType">context menu를 표시한 소스 컨트롤의 선택 표현 방식. SelectionType에 정의된 값 사용.</param>
        public string GetAttributeHtml(string menuType, string selType)
        {
            return GetAttributeHtml(menuType, (Hashtable)null, null, null, selType);
        }

        #region Hashtable 사용

        /// <summary>
        /// function 파라미터 Value를 Hashtable로 넘겨 받아서 Attribute 생성
        /// </summary>
        /// <param name="obj">context menu가 수행될 컨트롤 </param>
        /// <param name="menuType">context menu 형태</param>
        /// <param name="htValue">function에 전달할 argument 값</param>
        public string GetAttributeHtml(string menuType, Hashtable htValue)
        {
            return GetAttributeHtml(menuType, null, null, htValue, SelectionType.Default);
        }

        /// <summary>
        /// function 파라미터 Value를 Hashtable로 넘겨 받아서 Attribute 생성
        /// </summary>
        /// <param name="obj">context menu가 수행될 컨트롤 </param>
        /// <param name="menuType">context menu 형태</param>
        /// <param name="htValue">function에 전달할 argument 값</param>
        /// <param name="selType">context menu를 표시한 소스 컨트롤의 선택 표현 방식. SelectionType에 정의된 값 사용.</param>
        public string GetAttributeHtml(string menuType, Hashtable htValue, string selType)
        {
            return GetAttributeHtml(menuType, null, null, htValue, selType);
        }

        /// <summary>
        /// Disable 여부,Custom Function 목록,function 파라미터 Value를 Hashtable로 넘겨 받아서 Attribute 생성
        /// </summary>
        /// <param name="obj">context menu가 수행될 컨트롤 </param>
        /// <param name="menuType">context menu 형태</param>
        /// <param name="htDisabled">context Menu Item의 Disable 여부</param>
        /// <param name="htFunc">기본 function 이외의 custome function 목록</param>
        /// <param name="htValue">function에 전달할 argument 값</param>
        public string GetAttributeHtml(string menuType, Hashtable htDisabled, Hashtable htFunc, Hashtable htValue)
        {
            return GetAttributeHtml(menuType, htDisabled, htFunc, htValue, SelectionType.Default);
        }

        /// <summary>
        /// Disable 여부,Custom Function 목록,function 파라미터 Value를 Hashtable로 넘겨 받아서 Attribute 생성
        /// </summary>
        /// <param name="obj">context menu가 수행될 컨트롤 </param>
        /// <param name="menuType">context menu 형태</param>
        /// <param name="htDisabled">context Menu Item의 Disable 여부</param>
        /// <param name="htFunc">기본 function 이외의 custome function 목록</param>
        /// <param name="htValue">function에 전달할 argument 값</param>
        /// <param name="selType">context menu를 표시한 소스 컨트롤의 선택 표현 방식. SelectionType에 정의된 값 사용.</param>
        public string GetAttributeHtml(string menuType, Hashtable htDisabled, Hashtable htFunc, Hashtable htValue, string selType)
        {
            WebControl control = new WebControl(System.Web.UI.HtmlTextWriterTag.Div);

            AddAttribute(control, menuType, htDisabled, htFunc, htValue, selType);

            StringBuilder code = new StringBuilder(1024);

            foreach (string key in control.Attributes.Keys)
            {
                code.Append(string.Format(" {0}=\"{1}\"", key, control.Attributes[key]));
            }

            return code.ToString();
        }

        #endregion

        #region NameValueCollection 사용

        /// <summary>
        /// function 파라미터 Value를 NameValueCollection으로 넘겨 받아서 Attribute Append
        /// </summary>
        /// <param name="obj">context menu가 수행될 컨트롤 </param>
        /// <param name="menuType">context menu 형태</param>
        /// <param name="collValue">function에 전달할 argument 값</param>
        public string GetAttributeHtml(string menuType, NameValueCollection collValue)
        {
            return GetAttributeHtml(menuType, null, null, collValue, SelectionType.Default);
        }

        /// <summary>
        /// function 파라미터 Value를 NameValueCollection으로 넘겨 받아서 Attribute Append
        /// </summary>
        /// <param name="obj">context menu가 수행될 컨트롤 </param>
        /// <param name="menuType">context menu 형태</param>
        /// <param name="collValue">function에 전달할 argument 값</param>
        /// <param name="selType">context menu를 표시한 소스 컨트롤의 선택 표현 방식. SelectionType에 정의된 값 사용.</param>
        public string GetAttributeHtml(string menuType, NameValueCollection collValue, string selType)
        {
            return GetAttributeHtml(menuType, null, null, collValue, selType);
        }

        /// <summary>
        /// Disable 여부,Custom Function 목록,function 파라미터 Value를 NameValueCollection으로 넘겨 받아서 Attribute Append
        /// </summary>
        /// <param name="obj">context menu가 수행될 컨트롤 </param>
        /// <param name="menuType">context menu 형태</param>
        /// <param name="collDisabled">context Menu Item의 Disable 여부</param>
        /// <param name="collFunc">기본 function 이외의 custome function 목록</param>
        /// <param name="collValue">function에 전달할 argument 값</param>
        public string GetAttributeHtml(string menuType, NameValueCollection collDisabled, NameValueCollection collFunc, NameValueCollection collValue)
        {
            return GetAttributeHtml(menuType, collDisabled, collFunc, collValue, SelectionType.Default);
        }

        /// <summary>
        /// Disable 여부,Custom Function 목록,function 파라미터 Value를 NameValueCollection으로 넘겨 받아서 Attribute Append
        /// </summary>
        /// <param name="obj">context menu가 수행될 컨트롤 </param>
        /// <param name="menuType">context menu 형태</param>
        /// <param name="collDisabled">context Menu Item의 Disable 여부</param>
        /// <param name="collFunc">기본 function 이외의 custome function 목록</param>
        /// <param name="collValue">function에 전달할 argument 값</param>
        /// <param name="selType">context menu를 표시한 소스 컨트롤의 선택 표현 방식. SelectionType에 정의된 값 사용.</param>
        public string GetAttributeHtml(string menuType, NameValueCollection collDisabled, NameValueCollection collFunc, NameValueCollection collValue, string selType)
        {
            WebControl control = new WebControl(System.Web.UI.HtmlTextWriterTag.Div);

            AddAttribute(control, menuType, collDisabled, collFunc, collValue, selType);

            StringBuilder code = new StringBuilder(1024);

            foreach (string key in control.Attributes.Keys)
            {
                code.Append(string.Format(" {0}=\"{1}\"", key, control.Attributes[key]));
            }

            return code.ToString();
        }

        #endregion

        #endregion

        #region AddAttribute: WebControl에 Attribute 추가

        /// <summary>
        /// Attribute Append
        /// </summary>
        /// <param name="obj">context menu가 수행될 컨트롤 </param>
        /// <param name="menuType">context menu 형태</param>
        public void AddAttribute(WebControl webCtl, string menuType)
        {
            AddAttribute(webCtl, menuType, (Hashtable)null, null, null, SelectionType.Default);
        }

        /// <summary>
        /// Attribute Append
        /// </summary>
        /// <param name="obj">context menu가 수행될 컨트롤 </param>
        /// <param name="menuType">context menu 형태</param>
        /// <param name="selType">context menu를 표시한 소스 컨트롤의 선택 표현 방식. SelectionType에 정의된 값 사용.</param>
        public void AddAttribute(WebControl webCtl, string menuType, string selType)
        {
            AddAttribute(webCtl, menuType, (Hashtable)null, null, null, selType);
        }

        #region Hashtable 사용

        /// <summary>
        /// function 파라미터 Value를 Hashtable로 넘겨 받아서 Attribute Append
        /// </summary>
        /// <param name="obj">context menu가 수행될 컨트롤 </param>
        /// <param name="menuType">context menu 형태</param>
        /// <param name="htValue">function에 전달할 argument 값</param>
        public void AddAttribute(WebControl webCtl, string menuType, Hashtable htValue)
        {

            AddAttribute(webCtl, menuType, null, null, htValue, SelectionType.Default);
        }

        /// <summary>
        /// function 파라미터 Value를 Hashtable로 넘겨 받아서 Attribute Append
        /// </summary>
        /// <param name="obj">context menu가 수행될 컨트롤 </param>
        /// <param name="menuType">context menu 형태</param>
        /// <param name="htValue">function에 전달할 argument 값</param>
        /// <param name="selType">context menu를 표시한 소스 컨트롤의 선택 표현 방식. SelectionType에 정의된 값 사용.</param>
        public void AddAttribute(WebControl webCtl, string menuType, Hashtable htValue, string selType)
        {

            AddAttribute(webCtl, menuType, null, null, htValue, selType);
        }

        /// <summary>
        /// Disable 여부,Custom Function 목록,function 파라미터 Value를 Hashtable로 넘겨 받아서 Attribute Append
        /// </summary>
        /// <param name="obj">context menu가 수행될 컨트롤 </param>
        /// <param name="menuType">context menu 형태</param>
        /// <param name="htDisabled">context Menu Item의 Disable 여부</param>
        /// <param name="htFunc">기본 function 이외의 custome function 목록</param>
        /// <param name="htValue">function에 전달할 argument 값</param>
        public void AddAttribute(WebControl webCtl, string menuType, Hashtable htDisabled, Hashtable htFunc, Hashtable htValue)
        {
            this.AddAttribute(webCtl, menuType, htDisabled, htFunc, htValue, SelectionType.Default);
        }

        /// <summary>
        /// Disable 여부,Custom Function 목록,function 파라미터 Value를 Hashtable로 넘겨 받아서 Attribute Append
        /// </summary>
        /// <param name="obj">context menu가 수행될 컨트롤 </param>
        /// <param name="menuType">context menu 형태</param>
        /// <param name="htDisabled">context Menu Item의 Disable 여부</param>
        /// <param name="htFunc">기본 function 이외의 custome function 목록</param>
        /// <param name="htValue">function에 전달할 argument 값</param>
        /// <param name="selType">context menu를 표시한 소스 컨트롤의 선택 표현 방식. SelectionType에 정의된 값 사용.</param>
        public void AddAttribute(WebControl webCtl, string menuType, Hashtable htDisabled, Hashtable htFunc, Hashtable htValue, string selType)
        {

            string strDisabled = "";
            string strFunc = "";

            webCtl.Attributes.Add("menuType", menuType);

            if (htDisabled != null)
            {
                //commandName:value/commandName:value의 형태로 append함(ex: commandDisabled="Save:true/Cancel:false/")
                foreach (DictionaryEntry de in htDisabled)
                {

                    strDisabled += de.Key.ToString() + ":" + de.Value.ToString() + "/";
                }

                // Attribute Add
                webCtl.Attributes.Add("commandDisabled", strDisabled);
            }

            if (htFunc != null)
            {
                // commandName:function명/commandName:function명의 형태로 append함(ex: custFunc="Update:CustomUpdate/Cancel:CustomCancel/") 
                foreach (DictionaryEntry de in htFunc)
                {

                    strFunc += de.Key.ToString() + ":" + de.Value.ToString() + "/";
                }
                // Attribute Add
                webCtl.Attributes.Add("custFunc", strFunc);
            }

            if (htValue != null)
            {
                //__+commandName Attribute에 Append 함(ex: __ViewSPDM="'ViewSPDM-1',2,'ViewSPDM-3'")
                foreach (DictionaryEntry de in htValue)
                {
                    webCtl.Attributes.Add("__" + de.Key.ToString(), de.Value.ToString());
                }
            }

            if (string.IsNullOrEmpty(selType)) selType = SelectionType.Default;

            // 컨텍스트 메뉴 추가
            webCtl.Attributes.Add("oncontextmenu", "javascript:return ContextPop(this ,'" + selType + "');");
        }

        #endregion

        #region NameValueCollection 사용

        /// <summary>
        /// function 파라미터 Value를 NameValueCollection으로 넘겨 받아서 Attribute Append
        /// </summary>
        /// <param name="obj">context menu가 수행될 컨트롤 </param>
        /// <param name="menuType">context menu 형태</param>
        /// <param name="collValue">function에 전달할 argument 값</param>
        public void AddAttribute(WebControl webCtl, string menuType, NameValueCollection collValue)
        {

            AddAttribute(webCtl, menuType, null, null, collValue, SelectionType.None);
        }

        /// <summary>
        /// function 파라미터 Value를 NameValueCollection으로 넘겨 받아서 Attribute Append
        /// </summary>
        /// <param name="obj">context menu가 수행될 컨트롤 </param>
        /// <param name="menuType">context menu 형태</param>
        /// <param name="collValue">function에 전달할 argument 값</param>
        /// <param name="selType">context menu를 표시한 소스 컨트롤의 선택 표현 방식. SelectionType에 정의된 값 사용.</param>
        public void AddAttribute(WebControl webCtl, string menuType, NameValueCollection collValue, string selType)
        {

            AddAttribute(webCtl, menuType, null, null, collValue, selType);
        }

        /// <summary>
        /// Disable 여부,Custom Function 목록,function 파라미터 Value를 NameValueCollection으로 넘겨 받아서 Attribute Append
        /// </summary>
        /// <param name="obj">context menu가 수행될 컨트롤 </param>
        /// <param name="menuType">context menu 형태</param>
        /// <param name="collDisabled">context Menu Item의 Disable 여부</param>
        /// <param name="collFunc">기본 function 이외의 custome function 목록</param>
        /// <param name="collValue">function에 전달할 argument 값</param>
        public void AddAttribute(WebControl webCtl, string menuType, NameValueCollection collDisabled, NameValueCollection collFunc, NameValueCollection collValue)
        {
            this.AddAttribute(webCtl, menuType, collDisabled, collFunc, collValue, SelectionType.None);
        }

        /// <summary>
        /// Disable 여부,Custom Function 목록,function 파라미터 Value를 NameValueCollection으로 넘겨 받아서 Attribute Append
        /// </summary>
        /// <param name="obj">context menu가 수행될 컨트롤 </param>
        /// <param name="menuType">context menu 형태</param>
        /// <param name="collDisabled">context Menu Item의 Disable 여부</param>
        /// <param name="collFunc">기본 function 이외의 custome function 목록</param>
        /// <param name="collValue">function에 전달할 argument 값</param>
        /// <param name="selType">context menu를 표시한 소스 컨트롤의 선택 표현 방식. SelectionType에 정의된 값 사용.</param>
        public void AddAttribute(WebControl webCtl, string menuType, NameValueCollection collDisabled, NameValueCollection collFunc, NameValueCollection collValue, string selType)
        {
            string strDisabled = "";
            string strFunc = "";

            webCtl.Attributes.Add("menuType", menuType);


            if (collDisabled != null)
            {
                //commandName:value/commandName:value의 형태로 append함(ex: commandDisabled="Save:true/Cancel:false/")
                foreach (string strKey in collDisabled.AllKeys)
                {
                    strDisabled += strKey + ":" + collDisabled[strKey] + "/";
                }

                webCtl.Attributes.Add("commandDisabled", strDisabled);
            }

            if (collFunc != null)
            {
                // commandName:function명/commandName:function명의 형태로 append함(ex: custFunc="Update:CustomUpdate/Cancel:CustomCancel/") 
                foreach (string strKey in collFunc.AllKeys)
                {
                    strFunc += strKey + ":" + collFunc[strKey] + "/";
                }

                webCtl.Attributes.Add("custFunc", strFunc);
            }

            if (collValue != null)
            {
                //__+commandName Attribute에 Append 함(ex: __ViewSPDM="'ViewSPDM-1',2,'ViewSPDM-3'")
                foreach (string strKey in collValue.AllKeys)
                {
                    webCtl.Attributes.Add("__" + strKey, collValue[strKey]);
                }
            }

            if (string.IsNullOrEmpty(selType)) selType = SelectionType.Default;

            // 컨텍스트 메뉴 추가
            webCtl.Attributes.Add("oncontextmenu", "javascript:return ContextPop(this ,'" + selType + "');");
        }

        #endregion

        #endregion
    }

    public static class SelectionType
    {
        // 이미지,Span 등의 컨트롤이 선택시 Border로 표시
        public const string BorderType = "BorderType";
        // Table류의 컨트롤인 경우 BGColor로 표시
        public const string BGType = "BGType";
        // 컨트롤에 따라 자동으로 표시함(Table류인 경우 BGType, 이외의 경우 BorderType)
        public const string Default = "Default";
        // 선택여부 표시하지 않음
        public const string None = "None";
    }
}

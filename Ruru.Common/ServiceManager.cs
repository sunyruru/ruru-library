namespace Ruru.Common
{
    using System.Collections.Generic;
    using System.ServiceProcess;

    /// <summary>
    /// 
    /// </summary>
    public partial class ServiceManager
    {
        /// <summary>
        /// 
        /// </summary>
        public sealed class ServiceStatusClass
        {
            /// <summary>
            /// 
            /// </summary>
            public ServiceControllerStatus? Status;
            /// <summary>
            /// 
            /// </summary>
            public ServiceController Service;
        }

        /// <summary>
        /// 지정된 서버의 서비스 명을 찾아 있을 경우 서비스 상태를 반환
        /// </summary>
        /// <param name="sServiceName">서비스 이름</param>
        /// <param name="sServiceServer">지정된 서버</param>
        /// <returns><see cref="System.ServiceProcess.ServiceControllerStatus"/>서비스 상태 (Nillable)</returns>
        public static ServiceControllerStatus? ServiceStatus(string sServiceName, string sServiceServer)
        {
            ServiceControllerStatus? oResult = null;
            System.Collections.Generic.List<ServiceController> oServiceList = null;
            List<ServiceStatusClass> oResultList = null;

            try
            {
                oResultList = new List<ServiceStatusClass>();
                oServiceList = ServiceList(sServiceServer);

                foreach (ServiceController oItem in oServiceList)
                {
                    if (oItem != null && oItem.ServiceName.Equals(sServiceName, System.StringComparison.OrdinalIgnoreCase) == true)
                    {
                        oResult = oItem.Status;
                        break;
                    }
                }
            }
            catch (System.Exception) { }
            finally
            {
                if (oServiceList != null)
                {
                    oServiceList.Clear();
                    oServiceList = null;
                }
            }

            return oResult;
        }

        /// <summary>
        /// 지정된 서버의 서비스 목록을 반환
        /// </summary>
        /// <param name="sServiceServer">지정된 서버</param>
        /// <exception cref="System.ComponentModel.Win32Exception">An error occurred when accessing a system API.</exception>
        /// <returns><see cref="System.Collections.Generic.List&lt;ServiceController&gt;"/> 서비스 목록</returns>
        public static System.Collections.Generic.List<ServiceController> ServiceList(string sServiceServer)
        {
            return new System.Collections.Generic.List<ServiceController>(ServiceController.GetServices(sServiceServer));
        }
    }
}

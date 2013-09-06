namespace Ruru.Common
{
    /// <summary>
    /// Encapsulates a ResourceManager object used to acquire localized
    /// strings specific to this tool.
    /// </summary>
    public sealed class ResourceManagerHelper
    {
        private static System.Resources.ResourceManager resourceManager;

        /// <summary>
        /// 지정된 타입의 ResourceManager 생성
        /// </summary>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <param name="type"></param>
        public static void CreateResourceManager(string type)
        {
            try
            {
                resourceManager = new System.Resources.ResourceManager(type, System.Reflection.Assembly.GetExecutingAssembly());
            }
            catch (System.ArgumentNullException ex) { throw ex; }
            catch { throw; }
        }

        /// <summary>
        /// ResourceManager에서 문자열을 가져옴
        /// </summary>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.Resources.MissingManifestResourceException"></exception>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetString(string name)
        {
            string sResult = string.Empty;

            try
            {
                resourceManager.GetString(name, System.Globalization.CultureInfo.CurrentUICulture);
            }
            catch (System.ArgumentNullException ex) { throw ex; }
            catch (System.InvalidOperationException ex) { throw ex; }
            catch (System.Resources.MissingManifestResourceException ex) { throw ex; }
            catch { throw; }

            return sResult;
        }
    }
}

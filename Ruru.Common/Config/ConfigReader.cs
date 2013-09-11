using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ruru.Common.Exceptions;

namespace Ruru.Common.Config
{
    public static class ConfigReader
    {
        static object _lock;
        static Dictionary<string, ConfigSection> _sections;

        static ConfigReader()
        {
            _lock = new object();
            _sections = new Dictionary<string, ConfigSection>();
        }

        public static ConfigSection GetSection(string sectionName)
        {
            try
            {
                lock (_lock)
                {
                    if (!_sections.ContainsKey(sectionName))
                    {
                        ConfigSection section
                            = System.Configuration.ConfigurationManager.GetSection(sectionName) as ConfigSection;

                        // null이라도 일단 등록. 한번만 실제로 컨피그를 읽기 위함.
                        _sections.Add(sectionName, section);

                        if (section == null)
                        {
                            // 로그를 남기기 위해 예외 발생
                            throw new Exception(string.Format("{0} 섹션이 정의되지 않았습니다.", sectionName));
                        }
                    }
                }
                return _sections[sectionName];
            }
            catch (Exception ex)
            {
                LogManager.AddExceptionData(ref ex, "sectionName", sectionName);

                LogManager.WriteError(LogSourceType.ClassLibrary, ex);

                throw;
            }
            //$$소스분석에 의해 수정됨-끝.$$
        }

        public static string GetValue(string sectionName, string category, string key)
        {
            ConfigSection section = GetSection(sectionName);
            if (section == null)
            {
                return null;
            }
            else
            {
                return section.GetValue(category, key);
            }
        }

        public static string GetString(string sectionName, string category, string key)
        {
            return GetValue(sectionName, category, key);
            //try
            //{
            //    return GetValue(sectionName, category, key);
            //}
            ////$$소스분석에 의해 수정됨.$$
            //catch(Exception)
            //{
            //    //$$소스분석에 의해 주석처리됨.$$
            //    //return null;
            //    throw;
            //}
        }

        public static int GetInteger(string sectionName, string category, string key)
        {
            return int.Parse(GetValue(sectionName, category, key));
            //try
            //{
            //    return int.Parse(GetValue(sectionName, category, key));
            //}
            ////$$소스분석에 의해 수정됨.$$
            //catch(Exception)
            //{
            //    //$$소스분석에 의해 주석처리됨.$$
            //    //return 0;
            //    throw;
            //}
        }

        public static long GetLong(string sectionName, string category, string key)
        {
            return long.Parse(GetValue(sectionName, category, key));
            //try
            //{
            //    return long.Parse(GetValue(sectionName, category, key));
            //}
            ////$$소스분석에 의해 수정됨.$$
            //catch(Exception)
            //{
            //    //$$소스분석에 의해 주석처리됨.$$
            //    //return 0;
            //    throw;
            //}
        }

        public static bool GetBoolean(string sectionName, string category, string key)
        {
            return bool.Parse(GetValue(sectionName, category, key));
            //try
            //{
            //    return bool.Parse(GetValue(sectionName, category, key));
            //}
            ////$$소스분석에 의해 수정됨.$$
            //catch(Exception)
            //{
            //    //$$소스분석에 의해 주석처리됨.$$
            //    return false;
            //    throw;
            //}
        }
    }
}

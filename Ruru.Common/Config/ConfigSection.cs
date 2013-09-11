namespace Ruru.Common.Config
{
    using System;
    using System.Collections.Generic;
    using Ruru.Common.Exceptions;

    public class ConfigSection : System.Configuration.ConfigurationSection
    {
        string _sectionName;
        Dictionary<string, Dictionary<string, string>> _values;

        public ConfigSection()
        {
            _values = new Dictionary<string, Dictionary<string, string>>();
        }

        public string GetValue(string category, string key)
        {
            if (!string.IsNullOrEmpty(category) && !string.IsNullOrEmpty(key)
                && _values.ContainsKey(category) && _values[category].ContainsKey(key))
            {
                return _values[category][key];
            }
            else
            {
                throw new Exception(string.Format("{0} 섹션에 {1} 카테고리 또는 {2} 키가 정의되지 않았습니다.",
                    this.SectionName, category, key));
            }
            //$$소스분석에 의해 수정됨-끝.$$
        }

        public string SectionName
        {
            get { return _sectionName; }
        }

        protected override void DeserializeSection(System.Xml.XmlReader reader)
        {
            try
            {
                reader.MoveToElement();

                if (!reader.IsEmptyElement)
                {
                    Dictionary<string, string> subSectionValues = null;

                    while (reader.Read())
                    {
                        if (reader.NodeType == System.Xml.XmlNodeType.Element)
                        {
                            if (string.IsNullOrEmpty(_sectionName))
                            {
                                // 섹션 시작
                                _sectionName = reader.Name;
                            }
                            else if (subSectionValues == null)
                            {
                                // 새로운 서브 섹션 시작
                                subSectionValues = new Dictionary<string, string>();
                                _values.Add(reader.Name, subSectionValues);
                            }
                            else
                            {
                                // add 엘리먼트 시작
                                if (reader.AttributeCount > 0)
                                {
                                    string key = null;
                                    string value = null;
                                    while (reader.MoveToNextAttribute())
                                    {
                                        if (reader.Name == "key")
                                        {
                                            key = reader.Value;
                                        }
                                        else if (reader.Name == "value")
                                        {
                                            value = reader.Value;
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                                    {
                                        subSectionValues.Add(key, value);
                                    }
                                }
                            }
                        }
                        else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
                        {
                            subSectionValues = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteError(LogSourceType.ClassLibrary, ex);
            }
        }
    }
}

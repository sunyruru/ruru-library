namespace Ruru.Net
{
    using System;
    using System.Collections;
    using System.Text;

    using Ruru.Common.OS;


    /// <summary>
    /// DNS 레코드 쿼리 방식
    /// </summary>
    public enum DnsRecordType
    {
        /// <summary>
        /// SRV 레코드
        /// </summary>
        SRV,
        /// <summary>
        /// 호스트 레코드
        /// </summary>
        A,
        /// <summary>
        /// ALIAS 레코드
        /// </summary>
        CNAME
    }

    /// <summary>
    /// DNS 관련 명령어 실행
    /// </summary>
    public class DnsUtil
    {
        private string m_sipDomain = string.Empty;

        private ArrayList m_dnsQueries = new ArrayList();
        private ArrayList listViewMatch = new ArrayList();
        private ArrayList listViewNoMatch = new ArrayList();

        private ProcessLaunch m_process = new ProcessLaunch();

        /// <summary>
        /// 기본 생성자
        /// </summary>
        public DnsUtil()
        {
            this.m_sipDomain = string.Empty;
            this.m_dnsQueries.Clear();
            this.listViewMatch.Clear();
            this.listViewNoMatch.Clear();
        }

        /// <summary>
        /// DNS 쿼리로 OCS 관련 DNS 레코드 조회
        /// </summary>
        /// <param name="sDomain">OCS 도메인을 지정합니다.</param>
        /// <returns>결과를 ArrayList로 반환합니다.</returns>
        public ArrayList[] OcsDnsRun(string sDomain)
        {
            string log = string.Empty;

            if (string.IsNullOrEmpty(sDomain) == false)
            {
                // 0. Init Domain string
                this.m_sipDomain = sDomain[sDomain.Length - 1] == '.' ? sDomain : sDomain + ".";

                // 1. Init List
                this.m_dnsQueries.Clear();

                // 2. Set DNS query list
                this.m_dnsQueries.Add(new DnsQuery(DnsRecordType.SRV, "_sipinternaltls._tcp." + this.m_sipDomain));
                this.m_dnsQueries.Add(new DnsQuery(DnsRecordType.SRV, "_sipinternal._tcp." + this.m_sipDomain));
                this.m_dnsQueries.Add(new DnsQuery(DnsRecordType.SRV, "_sip._tls." + this.m_sipDomain));
                this.m_dnsQueries.Add(new DnsQuery(DnsRecordType.A, "sipinternal." + this.m_sipDomain));
                this.m_dnsQueries.Add(new DnsQuery(DnsRecordType.A, "sip." + this.m_sipDomain));
                this.m_dnsQueries.Add(new DnsQuery(DnsRecordType.A, "sipexternal." + this.m_sipDomain));

                // 3. Do DNS query
                log = DoDnsLookup();
            }

            return new ArrayList[2] { this.listViewMatch, this.listViewNoMatch };
        }

        private string DoDnsLookup()
        {
            StringBuilder sb = new StringBuilder(64);

            foreach (DnsQuery query in this.m_dnsQueries)
            {
                string str;
                switch (query.QueryType)
                {
                    case DnsRecordType.SRV:
                        str = "-querytype=SRV";
                        break;

                    case DnsRecordType.A:
                        str = "-querytype=A";
                        break;

                    default:
                        str = "";
                        break;
                }
                string text1 = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\cmd.exe";
                string str2 = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\nslookup.exe";
                this.m_process.Filename = str2;
                this.m_process.Args = str + " " + query.Record;
                this.m_process.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                this.m_process.UseShellExecute = false;
                sb.Append("\nAbout to issue query for record: " + query.Record + "\n");
                this.m_process.RunProcess();
                ParseDnsResult result = new ParseDnsResult(query, this.m_process.ProcessOutput);
                sb.Append("Finished DNS query. About to parse the results.\n");
                DnsResult dnsResult = result.m_dnsResult;

                if (dnsResult.foundRecord)
                {
                    this.listViewMatch.Add(dnsResult);
                }
                else
                {
                    this.listViewNoMatch.Add(dnsResult);
                }
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// DNS Query에 대한 데이터 구조
    /// </summary>
    public class DnsQuery
    {
        /// <summary>
        /// DNS Query의 타입을 지정합니다.
        /// </summary>
        public DnsRecordType QueryType
        {
            get { return m_queryType; }
            set { m_queryType = value; }
        }
        private DnsRecordType m_queryType;
        
        /// <summary>
        /// DNS Queyr의 레코드를 지정합니다.
        /// </summary>
        public string Record
        {
            get { return m_record; }
            set { m_record = value; }
        }
        private string m_record;

        /// <summary>
        /// 기본 생성자로 DNS QueryType과 레코드를 지정합니다.
        /// </summary>
        /// <param name="i_recordType"></param>
        /// <param name="i_record"></param>
        public DnsQuery(DnsRecordType i_recordType, string i_record)
        {
            this.m_queryType = i_recordType;
            this.m_record = i_record;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DnsResult
    {
        /// <summary>
        /// 찾은 레코드
        /// </summary>
        public bool foundRecord;
        /// <summary>
        /// 호스트명
        /// </summary>
        public string hostname;
        /// <summary>
        /// IP주소
        /// </summary>
        public string ip;
        /// <summary>
        /// 포트번호
        /// </summary>
        public string port;
        /// <summary>
        /// 중요도
        /// </summary>
        public string priority;
        /// <summary>
        /// 쿼리
        /// </summary>
        public DnsQuery query;
        /// <summary>
        /// Weight
        /// </summary>
        public string weight;
    }

    /// <summary>
    /// DNS 결과 분석
    /// </summary>
    public class ParseDnsResult
    {
        /// <summary>
        /// DNS 조회 결과
        /// </summary>
        public DnsResult m_dnsResult = new DnsResult();

        // Fields
        private const string AREC_ADDRESS_KEY = "Address:  ";
        private const string AREC_HOSTNAME_KEY = "Name:    ";
        private const string SRV_IP_ADDR_KEY = "internet address = ";
        private const string SRV_PORT_KEY = "port           = ";
        private const string SRV_PRIORITY_KEY = "priority       = ";
        private const string SRV_SRV_HOSTNAME_KEY = "svr hostname   = ";
        private const string SRV_WEIGHT_KEY = "weight         = ";

        /// <summary>
        /// DNS 조회 결과 분석
        /// </summary>
        /// <param name="i_dnsQuery"></param>
        /// <param name="i_nslookupResults"></param>
        public ParseDnsResult(DnsQuery i_dnsQuery, string i_nslookupResults)
        {
            if ((i_dnsQuery == null) || (i_nslookupResults == null))
            {
                throw new Exception("Invalid input while attempting to parse DNS results.");
            }
            this.m_dnsResult.query = i_dnsQuery;
            bool flag = false;
            if (this.m_dnsResult.query.QueryType == DnsRecordType.SRV)
            {
                this.ParseData("svr hostname   = ", i_nslookupResults, out flag, out this.m_dnsResult.hostname);
                if (flag)
                {
                    this.m_dnsResult.foundRecord = true;
                }
                else
                {
                    this.m_dnsResult.foundRecord = false;
                }
                if (this.m_dnsResult.foundRecord)
                {
                    this.ParseData("internet address = ", i_nslookupResults, out flag, out this.m_dnsResult.ip);
                    this.ParseData("port           = ", i_nslookupResults, out flag, out this.m_dnsResult.port);
                    this.ParseData("weight         = ", i_nslookupResults, out flag, out this.m_dnsResult.weight);
                    this.ParseData("priority       = ", i_nslookupResults, out flag, out this.m_dnsResult.priority);
                }
            }
            else
            {
                if (this.m_dnsResult.query.QueryType != DnsRecordType.A)
                {
                    throw new Exception("Only SRV records are implemented");
                }
                this.ParseData("Name:    ", i_nslookupResults, out flag, out this.m_dnsResult.hostname);
                if (flag)
                {
                    this.m_dnsResult.foundRecord = true;
                }
                else
                {
                    this.m_dnsResult.foundRecord = false;
                }
                if (this.m_dnsResult.foundRecord)
                {
                    this.ParseData("Address:  ", i_nslookupResults, out flag, out this.m_dnsResult.hostname);
                }
            }
        }

        private void ParseData(string i_delimiter, string i_nslookupResults, out bool o_found, out string o_result)
        {
            int num = i_nslookupResults.LastIndexOf(i_delimiter, StringComparison.OrdinalIgnoreCase);
            if (num != -1)
            {
                o_found = true;
                int num2 = (num + i_delimiter.Length) - 1;
                string str = i_nslookupResults.Substring(num2 + 1);
                string str2 = "\r\n";
                int index = str.IndexOf(str2, StringComparison.OrdinalIgnoreCase);
                o_result = str.Substring(0, index);
            }
            else
            {
                o_found = false;
                o_result = "";
            }
        }
    }
}

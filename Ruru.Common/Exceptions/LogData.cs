namespace Ruru.Common.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// 로그 데이터 구조
    /// </summary>
    public class LogData
    {
        bool _isStopped;
        string _description;
        DateTime _startTime;
        DateTime _endTime;
        Dictionary<string, string> _values;

        public LogData()
            : this(null)
        { }

        public LogData(string description)
        {
            _description = description;
            _values = new Dictionary<string, string>();

            // 타이머 시작
            this.StartTimer();
        }

        /// <summary>
        /// 값 추가.
        /// </summary>
        /// <param name="valueName">값 이름</param>
        /// <param name="value">값</param>
        public void AddValue(string valueName, object value)
        {
            if (string.IsNullOrEmpty(valueName)) return;

            // 기존 값 삭제
            if (_values.ContainsKey(valueName))
            {
                _values.Remove(valueName);
            }

            // 값 추가
            if (value == null)
            {
                _values.Add(valueName, "{null}");
            }
            else if (value == DBNull.Value)
            {
                _values.Add(valueName, "{DBNull}");
            }
            else
            {
                _values.Add(valueName, value.ToString());
            }
        }

        /// <summary>
        /// [Read-Only] 타이머 동작 여부
        /// </summary>
        public bool IsTimerStopped
        {
            get
            {
                return _isStopped;
            }
        }

        /// <summary>
        /// 타이머 시작.
        /// </summary>
        public void StartTimer()
        {
            _isStopped = false;
            _startTime = DateTime.Now;
            _endTime = DateTime.MaxValue;
        }

        /// <summary>
        /// 타이머 종료.
        /// </summary>
        public void StopTimer()
        {
            if (this.IsTimerStopped) return;
            _endTime = DateTime.Now;
            _isStopped = true;
        }

        /// <summary>
        /// 설명.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// [Read-Only] 값 목록 문자열.
        /// </summary>
        public string ValueStack
        {
            get
            {
                if (_values.Count == 0) return string.Empty;

                StringBuilder values = new StringBuilder(1024);
                foreach (string key in _values.Keys)
                {
                    values.AppendLine(string.Format("{0} = [{1}]", key, _values[key]));
                }
                return values.ToString();
            }
        }

        /// <summary>
        /// [Read-Only] 값 목록.
        /// </summary>
        public Dictionary<string, string> Values
        {
            get
            {
                if (_values == null) _values = new Dictionary<string, string>();
                return _values;
            }
        }

        /// <summary>
        /// [Read-Only] 타이머 시작 시각.
        /// </summary>
        public DateTime StartTime
        {
            get { return _startTime; }
        }

        /// <summary>
        /// [Read-Only] 타이머 종료 시각. Stop 상태가 아니면 현재 시각이 반환된다.
        /// </summary>
        public DateTime EndTime
        {
            get
            {
                if (this.IsTimerStopped)
                {
                    return _endTime;
                }
                else
                {
                    return DateTime.Now;
                }
            }
        }

        /// <summary>
        /// [Read-Only] 타이머 소요 시간. 밀리초 단위. Stop 상태가 아니면 현재 시각까지의 소요 시간이 반환된다.
        /// </summary>
        public long ElapsedMilliseconds
        {
            get
            {
                TimeSpan ts = new TimeSpan(this.EndTime.Ticks - this.StartTime.Ticks);
                return (long)ts.TotalMilliseconds;
            }
        }

        public override string ToString()
        {
            StringBuilder code = new StringBuilder(1024);
            code.AppendLine(this.Description);
            code.AppendLine(string.Format("StartTime: {0}", this.StartTime.ToString("yyyy-MM-dd HH:mm:ss.fff")));
            code.AppendLine(string.Format("EndTime: {0}", this.EndTime.ToString("yyyy-MM-dd HH:mm:ss.fff")));
            code.AppendLine(string.Format("ElapsedMilliseconds: {0}", this.ElapsedMilliseconds));

            if (_values.Count > 0)
            {
                code.AppendLine(this.ValueStack);
            }

            return code.ToString();
        }
    }
}

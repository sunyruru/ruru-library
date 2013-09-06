namespace Ruru.Messaging.MSMQ
{
    using System.Messaging;

    /// <summary>
    /// Microsoft Message Queue Repacking Class
    /// </summary>
    public class DBStoreManager : System.IDisposable, Ruru.Messaging.IStoreManager
    {
        #region Private variables
        private MessageQueue _msmq = null;
        private MessageQueueTransaction _tran = null;
        private StoreManagerMode _mode = StoreManagerMode.Write;
        #endregion


        #region Properties
        /// <summary>
        /// 연결 문자열
        /// </summary>
        public string ConnectionString { get; set; }
        #endregion


        #region Constructor, Destructor
        /// <summary>
        /// Default Ctor.
        /// </summary>
        public DBStoreManager() { }

        /// <summary>
        /// MSMQ Path를 입력받아 인스턴스 생성
        /// </summary>
        /// <param name="sQueuePath">MSMQ 경로</param>
        public DBStoreManager(string sQueuePath) : this(sQueuePath, StoreManagerMode.Write) { }

        /// <summary>
        /// MSMQ Path를 입력받아 인스턴스 생성
        /// </summary>
        /// <param name="sQueuePath">MSMQ 경로</param>
        /// <param name="oMode">MSMQ 사용 모드를 지정</param>
        public DBStoreManager(string sQueuePath, StoreManagerMode oMode)
        {
            _mode = oMode;
            ConnectionString = sQueuePath;
            GetConnection(ConnectionString, oMode);
        }
        #endregion


        #region Member Methods
        /// <summary>
        /// MSMQ 지정된 큐에 연결합니다.
        /// </summary>
        /// <param name="sQueuePath">MSMQ 경로</param>
        /// <param name="oMode">MSMQ 사용 모드를 지정</param>
        public object GetConnection(string sQueuePath, StoreManagerMode oMode)
        {
            try
            {
                if (MessageQueue.Exists(sQueuePath) == false)
                {
                    _msmq = MessageQueue.Create(sQueuePath, true);
                }
                else
                {
                    _msmq = new MessageQueue(
                        sQueuePath, false, true
                        , oMode == StoreManagerMode.Write ? QueueAccessMode.SendAndReceive : QueueAccessMode.ReceiveAndAdmin);
                }

                if (_msmq.Transactional)
                {
                    _tran = new MessageQueueTransaction();
                }
            }
            catch (System.Exception)
            {
                throw;
            }

            return _msmq;
        }

        /// <summary>
        /// MSMQ에 메시지를 보냅니다.
        /// </summary>
        /// <param name="oMsg"></param>
        public void AddMsg(object oMsg)
        {
            Message msg = null;

            try
            {
                if (oMsg is Message)
                {
                    msg = oMsg as Message;
                }
                else
                {
                    msg = new Message(oMsg);
                    msg.Label = "test";
                }

                if (_msmq.Transactional == true)
                {
                    if (_tran.Status != MessageQueueTransactionStatus.Pending)
                    {
                        _tran.Begin();
                    }
                    _msmq.Send(msg, _tran);
                    _tran.Commit();
                }
                else
                {
                    _msmq.Send(msg);
                }
            }
            catch (System.Exception)
            {
                if (_msmq.Transactional)
                {
                    _tran.Abort();
                }
                throw;
            }
        }

        /// <summary>
        /// MSMQ에서 메시지를 받아옵니다.
        /// </summary>
        /// <returns><see cref="System.Messaging.Message"/>메시지</returns>
        public Message GetMsg()
        {
            Message oMsg = null;

            try
            {
                if (_msmq.Transactional == true)
                {
                    if (_tran.Status != MessageQueueTransactionStatus.Pending)
                    {
                        _tran.Begin();
                    }
                    oMsg = _msmq.Receive(_tran);
                    _tran.Commit();
                }
                else
                {
                    oMsg = _msmq.Receive();
                }
            }
            catch (System.Exception)
            {
                if (_msmq.Transactional)
                {
                    _tran.Abort();
                }
                throw;
            }

            return oMsg;
        }
        #endregion


        #region Interface
        /// <summary>
        /// MSMQ 자원을 반환합니다.
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (this._msmq != null)
                {
                    this._msmq.Close();
                    this._msmq.Dispose();
                }

                if (this._tran != null)
                {
                    if (this._tran.Status == MessageQueueTransactionStatus.Pending)
                    {
                        this._tran.Commit();
                        this._tran.Dispose();
                    }
                    else
                    {
                        this._tran.Dispose();
                    }
                }
            }
            catch (System.Exception)
            {
                throw;
            }
            finally
            {
                this._msmq = null;
                this._tran = null;
            }
            System.GC.SuppressFinalize(this);
        }
        #endregion
    }
}

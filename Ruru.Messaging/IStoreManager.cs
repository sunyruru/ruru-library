namespace Ruru.Messaging
{
    interface IStoreManager
    {
        /// <summary>
        /// 저장소에 메시지를 추가합니다.
        /// </summary>
        /// <param name="oMsg">메시지</param>
        void AddMsg(object oMsg);

        /// <summary>
        /// 저장소 연결 문자열
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        /// Unmanaged Resource의 자원 해제
        /// </summary>
        void Dispose();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sQueuePath"></param>
        /// <param name="oMode"></param>
        /// <returns></returns>
        object GetConnection(string sQueuePath, StoreManagerMode oMode);
        System.Messaging.Message GetMsg();
    }
}

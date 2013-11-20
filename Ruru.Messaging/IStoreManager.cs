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
        /// 연결을 생성하거나 가져옵니다
        /// </summary>
        /// <param name="sQueuePath"></param>
        /// <param name="oMode"></param>
        /// <returns></returns>
        object GetConnection(string sQueuePath, StoreManagerMode oMode);
        /// <summary>
        /// 메시지를 가져옵니다
        /// </summary>
        System.Messaging.Message GetMsg();
    }
}

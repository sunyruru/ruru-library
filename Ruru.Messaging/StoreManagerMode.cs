namespace Ruru.Messaging
{
    /// <summary>
    /// MSMQ 사용 모드를 지정합니다.
    /// </summary>
    public enum StoreManagerMode
    {
        /// <summary>
        /// MSMQ 에서 메시지를 읽고 지우는 모드로 연결합니다.
        /// </summary>
        Read,
        /// <summary>
        /// MSMQ 에 메시지를 작성 모드로 연결합니다.
        /// </summary>
        Write
    }
}

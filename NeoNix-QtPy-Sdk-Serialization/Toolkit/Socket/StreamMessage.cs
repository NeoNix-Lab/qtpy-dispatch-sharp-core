namespace Neo.Quantower.Toolkit.Socket
{
    /// <summary>
    /// Basic message container used by <see cref="SocketManager"/> for
    /// length-prefixed JSON communication. Each message carries a command name
    /// and an optional payload string.
    /// </summary>
    public class StreamMessage
    {
        public string Command { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
    }
}

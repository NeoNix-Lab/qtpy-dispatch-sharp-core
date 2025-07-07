namespace socket_dispatcher_pseudo.Interfaces
{
    /// <summary>
    /// Represents the dynamic payload of a message.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Gets the logical name of the message type.
        /// </summary>
        string Title { get; }
    }

}

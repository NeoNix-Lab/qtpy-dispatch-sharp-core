namespace NeoNix_QtPy_Sdk_Serialization.Interfaces
{
    /// <summary>
    /// Defines the minimal contract for a serializable message.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// A unique name for this message type, used for routing.
        /// </summary>
        string Title { get; }
    }

}

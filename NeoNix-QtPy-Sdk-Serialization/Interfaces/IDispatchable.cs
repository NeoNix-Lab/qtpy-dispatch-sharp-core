using NeoNix_QtPy_Models;

namespace NeoNix_QtPy_Sdk_Serialization.Interfaces
{
    /// <summary>
    /// Represents a dispatchable handler for messages of type <typeparamref name="IMessage"/>.
    /// </summary>
    /// <typeparam name="">Type of message handled, must implement <see cref="IMessage"/>.</typeparam>
    public interface IDispatchable
    {
        /// <summary>
        /// Name of the message type (must match <see cref="IMessage.Title"/>).
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The message payload instance.
        /// </summary>
        IMessage Message { get; set; }

        /// <summary>
        /// Serialize the current <see cref="Message"/> to JSON.
        /// </summary>
        /// <returns>A JSON string representing the message.</returns>
        string ToJson();

        /// <summary>
        /// Executes <see cref="Callback"/> with <see cref="Message"/>, then raises <see cref="OnReceived"/>.
        /// </summary>
        void Invoke(string obj);
    }
}

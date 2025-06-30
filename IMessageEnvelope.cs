using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socket_dispatcher_pseudo
{
    /// <summary>
    /// Defines an envelope that wraps an <see cref="IMessage"/> and includes an <see cref="Action{IMessage}"/> callback.
    /// </summary>
    public interface IMessageEnvelope
    {
        /// <summary>
        /// Gets the name of the message type this envelope handles.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the contained message payload.
        /// </summary>
        IMessage Message { get; }

        /// <summary>
        /// Gets or sets the callback to invoke when the message is received.
        /// </summary>
        Action<IMessage> OnReceived { get; set; }

        /// <summary>
        /// Invokes the <see cref="OnReceived"/> callback, passing the wrapped message.
        /// </summary>
        void Invoke();

        /// <summary>
        /// Serializes this envelope to JSON.
        /// </summary>
        /// <returns>The JSON string representing this envelope.</returns>
        string ToJson();
    }
}

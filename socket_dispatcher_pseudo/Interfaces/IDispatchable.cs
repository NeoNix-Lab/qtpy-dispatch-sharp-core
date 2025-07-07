using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socket_dispatcher_pseudo.Interfaces
{
    /// <summary>
    /// Defines an envelope that wraps an <see cref="IMessage"/> and includes an <see cref="Action{IMessage}"/> callback.
    /// </summary>
    public interface IDispatchable<TMessage> where TMessage : IMessage
    {
        /// <summary>
        /// Gets the name of the message type this envelope handles.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the contained message payload.
        /// </summary>
        TMessage Message { get; }

        /// <summary>
        /// Invokes the <see cref="OnReceived"/> callback, passing the wrapped message.
        /// </summary>
        void Invoke();

        /// <summary>
        /// Serializes this envelope to JSON.
        /// </summary>
        /// <returns>The JSON string representing this envelope.</returns>
        string ToJson();

        Delegate GetCallback { get; }
    }
}

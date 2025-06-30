using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socket_dispatcher_pseudo
{
    /// <summary>
    /// Dispatches incoming JSON messages to registered envelopes based on message name.
    /// </summary>
    public class MessageDispatcher
    {
        private readonly Dictionary<string, IMessageEnvelope> _envelopes = new();

        /// <summary>
        /// Registers a callback for a given message name.
        /// </summary>
        /// <param name="messageName">The logical name of the message type.</param>
        /// <param name="onReceived">The action to invoke when that message is received.</param>
        public void Register(string messageName, Action<IMessage> onReceived)
        {
            var env = MessageEnvelope.Create(messageName, new Dictionary<string, object>(), onReceived);
            _envelopes[messageName] = env;
        }

        /// <summary>
        /// Parses raw JSON, locates the registered envelope, and invokes its callback.
        /// </summary>
        /// <param name="json">The incoming JSON string.</param>
        public void Dispatch(string json)
        {
            var envelope = MessageEnvelope.FromJson(json);
            if (_envelopes.TryGetValue(envelope.Name, out var registered))
            {
                envelope.OnReceived = registered.OnReceived;
                envelope.Invoke();
            }
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace socket_dispatcher_pseudo
{
    /// <summary>
    /// Concrete implementation of <see cref="IMessageEnvelope"/>, handling JSON deserialization.
    /// </summary>
    public class MessageEnvelope : IMessageEnvelope
    {
        /// <inheritdoc />
        public string Name { get; private set; }

        /// <inheritdoc />
        public IMessage Message { get; private set; }

        /// <inheritdoc />
        public Action<IMessage> OnReceived { get; set; }

        /// <summary>
        /// Creates an envelope instance with the specified values.
        /// </summary>
        /// <param name="name">The logical name of the message.</param>
        /// <param name="data">The dynamic data dictionary.</param>
        /// <param name="onReceived">Callback to invoke when received.</param>
        /// <returns>A new <see cref="MessageEnvelope"/>.</returns>
        public static MessageEnvelope Create(string name, Dictionary<string, object> data, Action<IMessage> onReceived)
        {
            return new MessageEnvelope
            {
                Name = name,
                Message = new DynamicMessage(name, data),
                OnReceived = onReceived
            };
        }

        /// <summary>
        /// Deserializes JSON into a <see cref="MessageEnvelope"/>, mapping the data to a <see cref="DynamicMessage"/>.
        /// </summary>
        /// <param name="json">The JSON string representing the envelope.</param>
        /// <returns>A new <see cref="MessageEnvelope"/> without an assigned callback.</returns>
        public static MessageEnvelope FromJson(string json)
        {
            var dto = JsonSerializer.Deserialize<MessageEnvelopeDto>(json);
            return new MessageEnvelope
            {
                Name = dto.Name,
                Message = new DynamicMessage(dto.Name, dto.Data)
            };
        }

        /// <inheritdoc />
        public string ToJson()
        {
            var dto = new MessageEnvelopeDto { Name = Name, Data = Message.Data };
            return JsonSerializer.Serialize(dto);
        }

        /// <inheritdoc />
        public void Invoke() => OnReceived?.Invoke(Message);

        // Private DTO class used solely for JSON deserialization.
        private class MessageEnvelopeDto
        {
            public string Name { get; set; }
            public Dictionary<string, object> Data { get; set; }
        }
    }

}

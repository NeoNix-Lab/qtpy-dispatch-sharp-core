using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            // Deserializza nel DTO che usa JsonExtensionData per catturare tutti i campi extra
            var dto = JsonConvert.DeserializeObject<MessageEnvelopeDto>(json)!;

            // Converte JObject Data in Dictionary<string, object>
            var dataDict = dto.Data.ToObject<Dictionary<string, object>>()!;

            return new MessageEnvelope
            {
                Name = dto.Name,
                Message = new DynamicMessage(dto.Name, dataDict)
            };
        }

        /// <inheritdoc />
        public string ToJson()
        {
            var dto = new MessageEnvelopeDto
            {
                Name = Name,
                Data = JObject.FromObject(Message.Data)
            };
            return JsonConvert.SerializeObject(dto);
        }

        /// <inheritdoc />
        public void Invoke() => OnReceived?.Invoke(Message);

        // Private DTO class used solely for JSON deserialization.
        [JsonObject(MemberSerialization.OptIn)]
        private class MessageEnvelopeDto
        {
            [JsonProperty("name", Required = Required.Always)]
            public string Name { get; set; }

            [JsonExtensionData]
            public JObject Data { get; set; } = default!;
        }
    }

}

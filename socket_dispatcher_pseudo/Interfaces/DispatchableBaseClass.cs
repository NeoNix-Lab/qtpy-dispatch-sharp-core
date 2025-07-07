using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace socket_dispatcher_pseudo.Interfaces
{
    /// <summary>
    /// Concrete implementation of <see cref="IDispatchable"/>, handling JSON deserialization.
    /// </summary>
    public class DispatchableBaseClass<TMessage> : IDispatchable<TMessage>
        where TMessage : IMessage
    {
        /// <inheritdoc />
        public string Name { get; private set; }
        /// <inheritdoc />
        public TMessage Message { get; private set; }

        /// <inheritdoc />
        public event EventHandler OnReceived;

        public Delegate GetCallback { get; private set; }


        public DispatchableBaseClass(TMessage message, Delegate @delegate)
        {
            this.Name = message.Title;
            this.Message = message;
            this.GetCallback = @delegate;
        }

        /// <summary>
        /// Deserializes JSON into a <see cref="DispatchableBaseClass"/>, mapping the data to a <see cref="DynamicMessage"/>.
        /// </summary>
        /// <param name="json">The JSON string representing the envelope.</param>
        /// <returns>A new <see cref="DispatchableBaseClass"/> without an assigned callback.</returns>
        public TMessage FromJson(string json) => JsonConvert.DeserializeObject<TMessage>(json)!;

        /// <inheritdoc />
        public string ToJson() => JsonConvert.SerializeObject(Message);

        public void Invoke() 
        {
            try
            {
                this.GetCallback.DynamicInvoke(Message);
                this.OnReceived?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message, ex);
            }
        }
    }
}

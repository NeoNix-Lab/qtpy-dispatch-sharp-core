using Newtonsoft.Json;

namespace NeoNix_QtPy_Sdk_Serialization.Interfaces
{
    //TODO: Conver generic type to i message

    /// <summary>
    /// Base class that ties together JSON serialization, a typed callback, and an event for any message type <typeparamref name="TMessage"/>.
    /// </summary>
    /// <typeparam name="TMessage">Type of message handled, must implement <see cref="IMessage"/>.</typeparam>
    public abstract class DispatchableBaseClass<TMessage> : IDispatchable
        where TMessage : IMessage
    {
        /// <inheritdoc/>
        public string Name { get; private set; }

        /// <inheritdoc/>
        public IMessage Message { get; set; }

        /// <inheritdoc/>
        public event EventHandler<TMessage> OnReceived;

        /// <inheritdoc/>
        public Action<TMessage> Callback { get; private set; }

        /// <summary>
        /// Creates a new dispatchable using the specified message and callback.
        /// </summary>
        /// <param name="message">
        /// An instance of <typeparamref name="TMessage"/>, whose <see cref="IMessage.Title"/>
        /// is used for routing.
        /// </param>
        /// <param name="callback">
        /// The action to execute when <see cref="Invoke"/> is called.
        /// </param>
        public DispatchableBaseClass(TMessage message, Action<TMessage> callback)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Callback = callback ?? throw new ArgumentNullException(nameof(callback));
            Name = message.Title;
        }

        public DispatchableBaseClass(TMessage message, Action<TMessage> callback, string name)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Callback = callback ?? throw new ArgumentNullException(nameof(callback));
            Name = name;
        }

        public DispatchableBaseClass(Action<TMessage> callback)
        {
            Callback = callback ?? throw new ArgumentNullException(nameof(callback));
            Name = typeof(TMessage).Name;
        }

        /// <inheritdoc/>
        public static IMessage FromJson(string json)
        {
            var msg = JsonConvert.DeserializeObject<TMessage>(json)
                      ?? throw new JsonSerializationException(
                          $"Cannot deserialize JSON to {typeof(TMessage).Name}");
            return msg;
        }

        public void UpdateMessage(IMessage message)
        {
            if (message is not TMessage typedMessage)
                throw new ArgumentException($"Message must be of type {typeof(TMessage).Name}", nameof(message));
            Message = typedMessage;
            Name = typedMessage.Title;
        }
        

        /// <inheritdoc/>
        public string ToJson() => JsonConvert.SerializeObject(Message);

        /// <inheritdoc/>
        public void Invoke()
        {
            try
            {
                // Execute the user-provided callback
                Callback((TMessage)Message);

                // Raise the OnReceived event
                OnReceived?.Invoke(this, (TMessage)Message);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Error invoking dispatchable for '{Name}'", ex);
            }
        }

    }
}

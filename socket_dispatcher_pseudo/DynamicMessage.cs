
using socket_dispatcher_pseudo.Interfaces;

namespace socket_dispatcher_pseudo
{
    /// <summary>
    /// Implements <see cref="IMessage"/> with a runtime-defined data dictionary.
    /// </summary>
    public class DynamicMessage : IMessage
    {
        /// <inheritdoc />
        public string Title { get; }

        /// <inheritdoc />
        public Dictionary<string, object> Data { get; }

        /// <summary>
        /// Creates a new dynamic message with the specified name and data.
        /// </summary>
        /// <param name="name">The logical name of the message.</param>
        /// <param name="data">A dictionary of message-specific data.</param>
        public DynamicMessage(string name, Dictionary<string, object> data)
        {
            Title = name;
            Data = data;
        }
    }
}

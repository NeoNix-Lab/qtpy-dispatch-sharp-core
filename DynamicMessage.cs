using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socket_dispatcher_pseudo
{
    /// <summary>
    /// Implements <see cref="IMessage"/> with a runtime-defined data dictionary.
    /// </summary>
    public class DynamicMessage : IMessage
    {
        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public Dictionary<string, object> Data { get; }

        /// <summary>
        /// Creates a new dynamic message with the specified name and data.
        /// </summary>
        /// <param name="name">The logical name of the message.</param>
        /// <param name="data">A dictionary of message-specific data.</param>
        public DynamicMessage(string name, Dictionary<string, object> data)
        {
            Name = name;
            Data = data;
        }
    }
}

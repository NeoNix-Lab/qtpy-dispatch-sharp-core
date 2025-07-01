using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socket_dispatcher_pseudo
{
    /// <summary>
    /// Represents the dynamic payload of a message.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Gets the logical name of the message type.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the key/value data dictionary for this message.
        /// </summary>
        Dictionary<string, object> Data { get; }
    }

}

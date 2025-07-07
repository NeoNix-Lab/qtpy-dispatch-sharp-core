using NeoNix_QtPy_Sdk_Serialization.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoNix_QtPy_Sdk_Serialization.Services
{
    /// <summary>
    /// Dispatches incoming JSON messages to registered envelopes based on message name.
    /// </summary>
    internal class MessageDispatcher
    {
        private readonly Dictionary<string, IDispatchable> _envelopes = new();

        /// <summary>
        /// Registers a callback for a given message name.
        /// </summary>
        public void Register(IDispatchable messagEnveloped)
        {
            if (!_envelopes.ContainsKey(messagEnveloped.Name))
                _envelopes[messagEnveloped.Name] = messagEnveloped;
            else
                throw new InvalidOperationException($"Envelope for message '{messagEnveloped.Name}' is already registered.");

        }

        /// <summary>
        /// Override a callback for a given message name.
        /// </summary>
        public void Override(IDispatchable messagEnveloped)
        {
            if (_envelopes.ContainsKey(messagEnveloped.Name))
                _envelopes[messagEnveloped.Name] = messagEnveloped;
            else
                throw new InvalidOperationException($"Envelope for message '{messagEnveloped.Name}' is not registered.");

        }

        /// <summary>
        /// Unegister a callback for a given message name.
        /// </summary>
        public void UnRegiste(IDispatchable messagEnveloped)
        {
            if (_envelopes.ContainsKey(messagEnveloped.Name))
                _envelopes.Remove(messagEnveloped.Name);
            else
                throw new InvalidOperationException($"Envelope for message '{messagEnveloped.Name}' is not registered.");

        }

        /// <summary>
        /// Parses raw JSON, locates the registered envelope, and invokes its callback.
        /// </summary>
        /// <param name="name">The incoming JSON string.</param>
        public void Dispatch(string name)
        {
            if (_envelopes.ContainsKey(name))
            {
                var envelope = _envelopes[name];
                envelope.Invoke();
            }
            else
            {
                throw new InvalidOperationException($"No registered envelope for message '{name}'.");
            }
        }

        public void Clear() => _envelopes.Clear();

    }

}

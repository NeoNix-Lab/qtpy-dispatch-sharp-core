using NeoNix_QtPy_Models;
using NeoNix_QtPy_Sdk_Serialization.Interfaces;
using NeoNix_QtPy_Sdk_Serialization.Services;
using System.Reflection;

namespace NeoNix_QtPy_Sdk_Serialization
{
    public sealed class MessageHub
    {
        static readonly Lazy<MessageHub> _instance = new(() => new MessageHub());
        internal readonly MessageDispatcher _dispatcher = new();
        private SocketManager _socketManager;
        public bool IsOperative { get; private set; } = false;

        public static MessageHub Instance => _instance.Value;

        private MessageHub() { }

        public async System.Threading.Tasks.Task Init(string host,
            int port,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default)
        {
            _socketManager = await SocketManager.ConnectAsync(host, port, timeout, cancellationToken);
            IsOperative = true;
            _ReadLoop();
        }

        private async System.Threading.Tasks.Task _ReadLoop()
        {
            while (true)
            {
                var incoming = await _socketManager.ReceiveAsync();
                //IMessage message = DispatchableBaseClass<IMessage>.FromJson(incoming);

                _dispatcher.Dispatch(incoming, "deprecated");
            }
        }

        public async System.Threading.Tasks.Task SendAsync(IDispatchable message,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default)
        {
            if (!IsOperative)
                throw new InvalidOperationException("MessageHub is not initialized. Call Init() first.");
            string Ime = message.ToJson();
            await _socketManager.SendAsync(Ime, timeout, cancellationToken);
        }

        public async System.Threading.Tasks.Task SendAsync(string message,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default)
        {
            if (!IsOperative)
                throw new InvalidOperationException("MessageHub is not initialized. Call Init() first.");
            await _socketManager.SendAsync(message, timeout, cancellationToken);
        }



        /// <summary>
        /// Registra un handler tipizzato.
        /// </summary>
        public void Register(IDispatchable dispatchable) => _dispatcher.Register(dispatchable);

        public void UnRegiste(IDispatchable messagEnveloped) => _dispatcher.UnRegiste(messagEnveloped);

        public void Override(IDispatchable messagEnveloped) => _dispatcher.Override(messagEnveloped);

        /// <summary>
        /// Resetta lo stato (per testing).
        /// </summary>
        public void Reset() => _dispatcher.Clear();

        public void Close()
        {
            _socketManager.Close();
            Reset();
            IsOperative = false;
        }
    }
}

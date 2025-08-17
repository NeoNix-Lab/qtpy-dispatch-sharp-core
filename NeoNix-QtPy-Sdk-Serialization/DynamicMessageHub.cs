using NeoNix_QtPy_Models;
using NeoNix_QtPy_Sdk_Serialization.Interfaces;
using NeoNix_QtPy_Sdk_Serialization.Services;
using System;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NeoNix_QtPy_Sdk_Serialization
{
    public sealed class DynamicMessageHub : IDisposable
    {
        static readonly Lazy<DynamicMessageHub> _instance = new(() => new DynamicMessageHub());
        public static DynamicMessageHub Instance => _instance.Value;

        private UnifiedSocketManager? _socketManager;
        private readonly MessageDispatcher _dispatcher = new();
        public bool IsOperative { get; private set; }

        private DynamicMessageHub() { }

        /// <summary>
        /// Inizializza usando UnifiedSocketManagerFactory (probe automatico server/client)
        /// </summary>
        public async Task InitWithFactoryAsync(
            string host,
            int port,
            int maxClients = 3,
            TimeSpan? defaultTimeout = null,
            TimeSpan? probeTimeout = null)
        {
            if (IsOperative)
                throw new InvalidOperationException("MessageHub già inizializzato");

            var ip = IPAddress.Parse(host);
            _socketManager = await UnifiedSocketManagerFactory.CreateAutoAsync(
                ip, port, maxClients, defaultTimeout, probeTimeout);

            AttachAndStart();
            IsOperative = true;
        }

        /// <summary>
        /// Inizializza manualmente specificando isServer
        /// </summary>
        public async Task InitManualAsync(
            string host,
            int port,
            bool isServer,
            int maxClients = 3,
            TimeSpan? defaultTimeout = null)
        {
            if (IsOperative)
                throw new InvalidOperationException("MessageHub già inizializzato");

            var ip = IPAddress.Parse(host);
            _socketManager = new UnifiedSocketManager(
                ip, port, isServer, maxClients, defaultTimeout);

            AttachAndStart();
            IsOperative = true;
        }

        private void AttachAndStart()
        {
            _socketManager!.MessageReceived += OnRawMessage;
            // avvia server o client
            _ = _socketManager.StartAsync();
        }

        private void OnRawMessage(object? sender, string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                string? title = doc.RootElement.GetProperty("Title").GetString();
                _dispatcher.Dispatch(title, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DynamicMessageHub] errore parsing: {ex}");
            }
        }

        public Task SendAsync(
            IDispatchable message,
            TimeSpan? timeout = null,
            CancellationToken? ct = null)
        {
            EnsureOperative();
            return _socketManager!.SendAsync(message.ToJson(), timeout, ct);
        }

        public Task SendAsync(
            string json,
            TimeSpan? timeout = null,
            CancellationToken? ct = null)
        {
            EnsureOperative();
            return _socketManager!.SendAsync(json, timeout, ct);
        }

        public void Register(IDispatchable handler) => _dispatcher.Register(handler);
        public void Unregister(IDispatchable handler) => _dispatcher.UnRegiste(handler);
        public void Override(IDispatchable handler) => _dispatcher.Override(handler);
        public void Reset() => _dispatcher.Clear();

        private void Close()
        {
            if (_socketManager != null)
            {
                _socketManager.MessageReceived -= OnRawMessage;
                _socketManager.Dispose();
                _socketManager = null;
            }
            Reset();
            IsOperative = false;
        }

        private void EnsureOperative()
        {
            if (!IsOperative || _socketManager == null)
                throw new InvalidOperationException("MessageHub non inizializzato. Chiama InitWithFactoryAsync() o InitManualAsync() prima.");
        }

        public void Dispose() => Close();
    }
}

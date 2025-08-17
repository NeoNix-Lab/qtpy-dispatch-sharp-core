using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NeoNix_QtPy_Sdk_Serialization.Services 
{ 
    public class UnifiedSocketManager : IDisposable
    {
        // Server fields
        private TcpListener? _listener;
        private readonly List<TcpClient> _clients = new();

        // Client field
        private TcpClient? _client;
        private NetworkStream? _stream;

        // Common config
        public IPAddress Address { get; }
        public int Port { get; }
        public bool IsServer { get; }
        public int MaxClients { get; }
        public bool IsConnected => IsServer
            ? _listener != null
            : _client?.Connected ?? false;

        // Cancellation
        private readonly CancellationTokenSource _cts;

        // Evento per messaggi ricevuti
        public event EventHandler<string>?   MessageReceived;

        public UnifiedSocketManager(
            IPAddress address,
            int port,
            bool isServer,
            int maxClients = 3,
            TimeSpan? defaultTimeout = null)
        {
            Address = address;
            Port = port;
            IsServer = isServer;
            MaxClients = maxClients;
            _cts = new CancellationTokenSource();
            DefaultTimeout = defaultTimeout ?? TimeSpan.FromSeconds(10);
        }

        public TimeSpan DefaultTimeout { get; }

        public async Task StartAsync()
        {
            if (IsServer) await StartServerAsync();
            else await ConnectAsync();
        }

        // ========================
        // === SERVER METHODS ====
        // ========================
        private async Task StartServerAsync()
        {
            _listener = new TcpListener(Address, Port);
            _listener.Start();
            Console.WriteLine($"[Server] Listening on {Address}:{Port}");

            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    var tcp = await _listener
                        .AcceptTcpClientAsync()
                        .WaitAsync(_cts.Token);

                    if (_clients.Count >= MaxClients)
                    {
                        Console.WriteLine("[Server] Max clients reached, rejecting");
                        tcp.Close();
                        continue;
                    }

                    _clients.Add(tcp);
                    _ = Task.Run(() => HandleClientAsync(tcp, _cts.Token));
                }
            }
            catch (OperationCanceledException) { /* shutdown */ }
        }

        private async Task HandleClientAsync(TcpClient tcp, CancellationToken token)
        {
            Console.WriteLine($"[Server] Client connected: {tcp.Client.RemoteEndPoint}");
            using var stream = tcp.GetStream();

            try
            {
                while (!token.IsCancellationRequested && tcp.Connected)
                {
                    var msg = await ReceiveAsync(stream, token);
                    if (msg == null) break;
                    MessageReceived?.Invoke(this, msg);
                }
            }
            catch (TimeoutException tx)
            {
                Console.WriteLine($"[Server] Timeout: {tx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Server] Error: {ex.Message}");
            }
            finally
            {
                _clients.Remove(tcp);
                tcp.Close();
                Console.WriteLine("[Server] Client disconnected");
            }
        }

        // ========================
        // === CLIENT METHODS ====
        // ========================
        public async Task ConnectAsync(TimeSpan? timeout = null)
        {
            _client = new TcpClient();
            var ct = _cts.Token;
            var to = timeout ?? DefaultTimeout;

            using var ctsLink = CancellationTokenSource.CreateLinkedTokenSource(ct);
            ctsLink.CancelAfter(to);

            try
            {
                await _client.ConnectAsync(Address, Port)
                             .WaitAsync(ctsLink.Token);
                _stream = _client.GetStream();
                Console.WriteLine($"[Client] Connected to {Address}:{Port}");
                Console.WriteLine($"[Stream] Stream {_stream}");

                // loop ricezione
                while (!_cts.Token.IsCancellationRequested && _client.Connected)
                {
                    var msg = await ReceiveAsync(_stream, _cts.Token);
                    if (msg == null) break;
                    MessageReceived?.Invoke(this, msg);
                }
            }
            catch (OperationCanceledException) when (ctsLink.IsCancellationRequested)
            {
                _client.Close();
                throw new TimeoutException($"Connect timed out after {to}");
            }
        }

        // ========================
        // === SEND & RECV =======
        // ========================
        public async Task SendAsync(
            string json,
            TimeSpan? timeout = null,
            CancellationToken? externalToken = null,
            int clientindex = 0)
        {
            var token = externalToken ?? _cts.Token;
            var to = timeout ?? DefaultTimeout;
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
            cts.CancelAfter(to);

            var data = Encoding.UTF8.GetBytes(json);
            var len = BitConverter.GetBytes(data.Length);

            try
            {
                // attenzione: _stream può provenire da client o da server->tcp
                var st = _stream ?? _clients[clientindex].GetStream() ?? throw new InvalidOperationException("No stream available");
                await st.WriteAsync(len, 0, 4, cts.Token);
                await st.WriteAsync(data, 0, data.Length, cts.Token);
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException($"Send timed out after {to}");
            }
        }

        private static async Task<string?> ReceiveAsync(
            NetworkStream stream,
            CancellationToken token)
        {
            // read prefix
            var prefix = new byte[4];
            await ReadExactAsync(stream, prefix, token);
            int msgLen = BitConverter.ToInt32(prefix, 0);

            // read payload
            var buf = new byte[msgLen];
            await ReadExactAsync(stream, buf, token);
            return Encoding.UTF8.GetString(buf);
        }

        private static async Task ReadExactAsync(
            NetworkStream stream,
            byte[] buffer,
            CancellationToken token)
        {
            int read = 0;
            while (read < buffer.Length)
            {
                int n = await stream.ReadAsync(buffer, read, buffer.Length - read, token);
                if (n == 0) throw new SocketException();
                read += n;
            }
        }

        // ========================
        // === SHUTDOWN & DISPOSE =
        // ========================
        public void Close()
        {
            _cts.Cancel();

            if (_client != null)
            {
                _client.Close();
                _stream = null;
            }

            if (_listener != null)
            {
                foreach (var tcp in _clients) tcp.Close();
                _clients.Clear();
                _listener.Stop();
            }
        }

        public void Dispose() => Close();
    }

    public static class UnifiedSocketManagerFactory
    {
        public static async Task<UnifiedSocketManager> CreateAutoAsync(
            IPAddress address,
            int port,
            int maxClients = 3,
            TimeSpan? defaultTimeout = null,    // Timeout “principale” (client/server)
            TimeSpan? probeTimeout = null)    // Timeout solo per il probe
        {
            // Se non specificato, uso 10s per le operazioni principali, 1s per il probe
            var dt = defaultTimeout ?? TimeSpan.FromSeconds(10);
            var pt = probeTimeout ?? TimeSpan.FromSeconds(1);

            bool canConnect = await ProbeAsync(address, port, pt);
            var isServer = !canConnect;

            return new UnifiedSocketManager(address, port, isServer, maxClients, dt);
        }

        private static async Task<bool> ProbeAsync(
            IPAddress address,
            int port,
            TimeSpan timeout)
        {
            using var tcp = new TcpClient();
            using var cts = new CancellationTokenSource(timeout);
            try
            {
                await tcp.ConnectAsync(address, port).WaitAsync(cts.Token);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }


}

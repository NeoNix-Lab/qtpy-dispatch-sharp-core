using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Neo.Quantower.Toolkit.Socket
{
    /// <summary>
    /// Handles both TCP server and client roles with persistent socket communication.
    /// Supports JSON-serialized <see cref="StreamMessage"/> exchange with length-prefixed framing.
    /// </summary>
    public class SocketManager : IDisposable
    {
        public TcpListener? Listener { get; private set; }
        public TcpClient? Client { get; private set; }
        public bool IsServer { get; private set; }
        public bool IsConnected => Client?.Connected ?? false;
        public IPAddress Ip { get; init; }
        public int Port { get; init; }
        public int MaxClient { get; init; } = 3;

        private readonly List<TcpClient> _tcpClients = new();
        public List<TcpClient> TcpClients => _tcpClients;
        private NetworkStream? _stream;
        public NetworkStream? Stream => _stream;
        private readonly CancellationToken _cancellationToken;
        private readonly CancellationTokenSource _cts;
        public event EventHandler<StreamMessage>? MessageReceived;

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketManager"/> class.
        /// </summary>
        /// <param name="address">The IP address to bind or connect to.</param>
        /// <param name="port">The port to bind or connect to.</param>
        /// <param name="isServer">If true acts as server, otherwise client.</param>
        /// <param name="cts">Optional cancellation token source for graceful shutdown.</param>
        /// <param name="maxClients">Maximum clients for the server (default 3).</param>
        public SocketManager(IPAddress address, int port, bool isServer, CancellationTokenSource? cts = null, int maxClients = 3)
        {
            Ip = address;
            Port = port;
            IsServer = isServer;
            _cts = cts ?? new CancellationTokenSource();
            _cancellationToken = _cts.Token;
            MaxClient = maxClients;
        }

        /// <summary>
        /// Starts the socket manager based on the specified role (client or server).
        /// </summary>
        public Task StartAsync() => IsServer ? StartServerAsync() : ConnectAsync();

        private async Task StartServerAsync()
        {
            if (Listener != null)
                throw new InvalidOperationException("Server is already started.");

            Listener = new TcpListener(Ip, Port);
            Listener.Start();
            Console.WriteLine($"Server listening on {Ip}:{Port}");

            while (!_cancellationToken.IsCancellationRequested)
            {
                var client = await Listener.AcceptTcpClientAsync(_cancellationToken).ConfigureAwait(false);

                if (_tcpClients.Count >= MaxClient)
                {
                    Console.WriteLine("Max client limit reached. Rejecting new connection.");
                    client.Close();
                    continue;
                }

                _tcpClients.Add(client);
                _ = Task.Run(() => HandleClientAsync(client, _cancellationToken), _cancellationToken);
            }
        }

        private async Task HandleClientAsync(TcpClient client, CancellationToken ct)
        {
            Console.WriteLine($"[+] Connected: {client.Client.RemoteEndPoint}");

            try
            {
                using var stream = client.GetStream();
                while (!ct.IsCancellationRequested)
                {
                    var message = await ReceiveMessageAsync(stream).ConfigureAwait(false);
                    if (message == null) break;
                    MessageReceived?.Invoke(this, message);
                    Console.WriteLine($"[MSG] {message.Command}: {message.Payload}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error with client {client.Client.RemoteEndPoint}: {ex.Message}");
            }
            finally
            {
                _tcpClients.Remove(client);
                client.Close();
                Console.WriteLine("[-] Client disconnected");
            }
        }

        private async Task ConnectAsync()
        {
            Client = new TcpClient();
            await Client.ConnectAsync(Ip, Port).ConfigureAwait(false);
            _stream = Client.GetStream();

            while (!_cancellationToken.IsCancellationRequested)
            {
                var message = await ReceiveMessageAsync(_stream).ConfigureAwait(false);
                if (message == null) break;
                MessageReceived?.Invoke(this, message);
                Console.WriteLine($"[MSG] {message.Command}: {message.Payload}");
            }

            Console.WriteLine("Connected to server.");
        }

        /// <summary>
        /// Sends a JSON message to the stream with a 4-byte length prefix.
        /// </summary>
        public static async Task SendMessageAsync(NetworkStream stream, StreamMessage msg, CancellationToken ct)
        {
            var json = JsonSerializer.Serialize(msg);
            var data = Encoding.UTF8.GetBytes(json);
            var len = BitConverter.GetBytes(data.Length);

            await stream.WriteAsync(len, ct).ConfigureAwait(false);
            await stream.WriteAsync(data, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Receives a length-prefixed JSON message from the stream.
        /// </summary>
        public static async Task<StreamMessage?> ReceiveMessageAsync(NetworkStream stream)
        {
            var lenBuffer = new byte[4];
            int read = await stream.ReadAsync(lenBuffer).ConfigureAwait(false);
            if (read == 0) return null;

            int msgLength = BitConverter.ToInt32(lenBuffer, 0);
            var msgBuffer = new byte[msgLength];
            read = await stream.ReadAsync(msgBuffer).ConfigureAwait(false);
            if (read == 0) return null;

            var json = Encoding.UTF8.GetString(msgBuffer);
            return JsonSerializer.Deserialize<StreamMessage>(json);
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();

            if (Client != null)
            {
                Client.Close();
                Client = null;
                Console.WriteLine("Client disconnected.");
            }
            if (Listener != null)
            {
                Listener.Stop();
                Listener = null;
                Console.WriteLine("Server stopped.");
            }

            foreach (var client in _tcpClients)
                client.Close();
            _tcpClients.Clear();
        }
    }
}

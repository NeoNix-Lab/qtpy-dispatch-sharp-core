using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace socket_dispatcher_pseudo
{
    /// <summary>
    /// Wraps low-level TCP socket operations for JSON messaging,
    /// with timeout that closes the connection on expiry.
    /// </summary>
    public class SocketManager
    {
        private readonly Socket _socket;

        private SocketManager(Socket socket) => _socket = socket;

        /// <summary>
        /// Opens a connection to the specified host and port.
        /// If the timeout elapses before connection completes, the socket is closed and a TimeoutException is thrown.
        /// </summary>
        public static async Task<SocketManager> ConnectAsync(
            string host,
            int port,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default)
        {
            Socket sock = null!;
            CancellationTokenSource? cts = null;

            try
            {
                // resolve DNS
                var ip = await Dns.GetHostEntryAsync(host).ConfigureAwait(false);
                var end = new IPEndPoint(ip.AddressList[0], port);

                // create socket
                sock = new Socket(end.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // prepare timeout token if requested
                if (timeout.HasValue)
                {
                    cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                    cts.CancelAfter(timeout.Value);
                }
                var token = cts?.Token ?? cancellationToken;

                // attempt connect
                await sock.ConnectAsync(end, token).ConfigureAwait(false);
                return new SocketManager(sock);
            }
            catch (OperationCanceledException) when (timeout.HasValue)
            {
                sock?.Close();
                throw new TimeoutException($"ConnectAsync timed out after {timeout}");
            }
            finally
            {
                cts?.Dispose();
            }
        }

        /// <summary>
        /// Sends a raw JSON string over the socket, with 4-byte little-endian length prefix.
        /// If the timeout elapses, the socket is closed and a TimeoutException is thrown.
        /// </summary>
        public async Task SendAsync(
            string json,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default)
        {
            using var cts = timeout.HasValue
                ? CancellationTokenSource.CreateLinkedTokenSource(cancellationToken)
                : null;
            if (timeout.HasValue)
                cts!.CancelAfter(timeout.Value);
            var token = cts?.Token ?? cancellationToken;

            var payload = Encoding.UTF8.GetBytes(json);
            var prefix = BitConverter.GetBytes(payload.Length); // little-endian

            try
            {
                await _socket.SendAsync(prefix, SocketFlags.None, token).ConfigureAwait(false);
                await _socket.SendAsync(payload, SocketFlags.None, token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (timeout.HasValue)
            {
                Close();
                throw new TimeoutException($"SendAsync timed out after {timeout}");
            }
        }

        private async Task ReadExactAsync(
            byte[] buffer,
            int offset,
            int count,
            CancellationToken token)
        {
            int read = 0;
            while (read < count)
            {
                int n = await _socket
                    .ReceiveAsync(new ArraySegment<byte>(buffer, offset + read, count - read),
                                  SocketFlags.None,
                                  token)
                    .ConfigureAwait(false);
                if (n == 0)
                    throw new SocketException(); // connection closed
                read += n;
            }
        }

        /// <summary>
        /// Receives a JSON string from the socket with 4-byte little-endian length prefix.
        /// If the timeout elapses, the socket is closed and a TimeoutException is thrown.
        /// </summary>
        public async Task<string> ReceiveAsync(
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default)
        {
            using var cts = timeout.HasValue
                ? CancellationTokenSource.CreateLinkedTokenSource(cancellationToken)
                : null;
            if (timeout.HasValue)
                cts!.CancelAfter(timeout.Value);
            var token = cts?.Token ?? cancellationToken;

            try
            {
                // read 4-byte length prefix
                var lenBuf = new byte[4];
                await ReadExactAsync(lenBuf, 0, 4, token).ConfigureAwait(false);
                int msgLen = BitConverter.ToInt32(lenBuf, 0);

                // read payload
                var dataBuf = new byte[msgLen];
                await ReadExactAsync(dataBuf, 0, msgLen, token).ConfigureAwait(false);

                return Encoding.UTF8.GetString(dataBuf);
            }
            catch (OperationCanceledException) when (timeout.HasValue)
            {
                Close();
                throw new TimeoutException($"ReceiveAsync timed out after {timeout}");
            }
        }

        /// <summary>
        /// Gracefully shuts down and closes the connection.
        /// </summary>
        public void Close()
        {
            try { _socket.Shutdown(SocketShutdown.Both); } catch { }
            _socket.Close();
        }
    }
}

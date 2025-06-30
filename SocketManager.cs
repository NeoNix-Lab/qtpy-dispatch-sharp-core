using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace socket_dispatcher_pseudo
{
    /// <summary>
    /// Wraps low-level TCP socket operations for JSON messaging.
    /// </summary>
    public class SocketManager
    {
        private readonly Socket _socket;

        private SocketManager(Socket socket) => _socket = socket;

        /// <summary>
        /// Opens a connection to the specified host and port.
        /// </summary>
        public static async Task<SocketManager> ConnectAsync(string host, int port)
        {
            var ip = await Dns.GetHostEntryAsync(host);
            var end = new IPEndPoint(ip.AddressList[0], port);
            var sock = new Socket(end.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            await sock.ConnectAsync(end);
            return new SocketManager(sock);
        }

        /// <summary>
        /// Sends a raw JSON string over the socket.
        /// </summary>
        public Task SendAsync(string json)
        {
            var buffer = Encoding.UTF8.GetBytes(json);
            return _socket.SendAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
        }

        /// <summary>
        /// Receives a JSON string from the socket. Assumes messages are under 4096 bytes.
        /// </summary>
        public async Task<string> ReceiveAsync()
        {
            var buffer = new byte[4096];
            int count = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
            return Encoding.UTF8.GetString(buffer, 0, count);
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

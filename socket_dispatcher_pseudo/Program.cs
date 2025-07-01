using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace socket_dispatcher_pseudo
{
    /// <summary>
    /// Demonstrates connecting, sending, receiving, and dispatching messages.
    /// </summary>
    public class Program
    {
        public static async Task Main()
        {
            // Connect to the server
            var manager = await SocketManager.ConnectAsync("localhost", 9000);

            // Set up dispatcher and register handlers
            var dispatcher = new MessageDispatcher();
            dispatcher.Register(TradingEnvelopes.NewOrder);
            dispatcher.Register(TradingEnvelopes.PriceUpdate);
            dispatcher.Register(TradingEnvelopes.AccountBalance);

            // Example: send a NewOrder envelope to the server
            await manager.SendAsync(TradingEnvelopes.NewOrder.ToJson());

            // Receive loop
            while (true)
            {
                var incoming = await manager.ReceiveAsync();
                dispatcher.Dispatch(incoming);
            }
        }
    }
}

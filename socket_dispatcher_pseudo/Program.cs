using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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

        //static async Task Main()
        //{
        //    // 1. Initialize socket + schema loader
        //    var socketSvc = new SocketClientService("localhost", 9999, timeoutMs: 5000);
        //    socketSvc.Connect();

        //    var schemaMgr = new SchemaManager(@"C:\path\to\schemas");

        //    // 2. Receive, validate, wrap, then dispatch
        //    while (true)
        //    {
        //        string rawJson;
        //        try
        //        {
        //            rawJson = socketSvc.ReceiveRawJson();
        //        }
        //        catch (SocketException)
        //        {
        //            Console.WriteLine("Connection closed by peer");
        //            break;
        //        }

        //        IMessage msg;
        //        try
        //        {
        //            msg = schemaMgr.ParseAndValidate(rawJson);
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Bad message: {ex.Message}");
        //            continue;
        //        }

        //        HandleMessage(msg);
        //    }
        //}

        //static void HandleMessage(IMessage msg)
        //{
        //    Console.WriteLine($"=== Received '{msg.Name}' ===");
        //    foreach (var kv in msg.Data)
        //        Console.WriteLine($"  {kv.Key} = {kv.Value}");
        //    // optionally dispatch to stronger-typed handlers:
        //    if (msg.Name == "UserInfo")
        //    {
        //        var name = (string)msg.Data["username"];
        //        var age = Convert.ToInt32(msg.Data["age"]);
        //        Console.WriteLine($" Hello {name}, age {age}!");
        //    }
        //}
    }
}

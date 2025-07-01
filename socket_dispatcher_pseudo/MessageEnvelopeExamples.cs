using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socket_dispatcher_pseudo
{
    /// <summary>
    /// Contains three example message envelopes for trading
    /// </summary>
    public static class TradingEnvelopes
    {
        // 1) New Order: sends a new buy/sell order
        public static readonly IMessageEnvelope NewOrder = MessageEnvelope.Create(
            name: "NewOrder",
            data: new Dictionary<string, object>
            {
            { "OrderId", Guid.NewGuid().ToString() },
            { "Symbol", "AAPL" },
            { "Side", "BUY" },            // BUY or SELL
            { "Quantity", 100 },
            { "Price", 172.35 }
            },
            onReceived: msg =>
            {
                Console.WriteLine($"[NewOrder] Received order {msg.Data["OrderId"]} for {msg.Data["Quantity"]}×{msg.Data["Symbol"]}");
            }
        );

        // 2) Price Update: notifies of a market price update
        public static readonly IMessageEnvelope PriceUpdate = MessageEnvelope.Create(
            name: "PriceUpdate",
            data: new Dictionary<string, object>
            {
            { "Symbol", "AAPL" },
            { "Bid", 172.30 },
            { "Ask", 172.40 },
            { "Timestamp", DateTime.UtcNow }
            },
            onReceived: msg =>
            {
                Console.WriteLine($"[PriceUpdate] {msg.Data["Symbol"]}: Bid={msg.Data["Bid"]}, Ask={msg.Data["Ask"]}");
            }
        );

        // 3) Account Balance: portfolio status after execution
        public static readonly IMessageEnvelope AccountBalance = MessageEnvelope.Create(
            name: "AccountBalance",
            data: new Dictionary<string, object>
            {
            { "AccountId", "ACC12345" },
            { "Cash", 25000.50 },
            { "Positions", new Dictionary<string, object>
                {
                    { "AAPL", 150 },
                    { "MSFT", 75 }
                }
            }
            },
            onReceived: msg =>
            {
                Console.WriteLine($"[AccountBalance] Cash={msg.Data["Cash"]}, Positions:");
                var positions = msg.Data["Positions"] as Dictionary<string, object>;
                foreach (var kv in positions)
                    Console.WriteLine($"    {kv.Key}: {kv.Value}");
            }
        );
    }

}

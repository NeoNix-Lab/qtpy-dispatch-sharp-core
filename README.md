# SocketDispatcher

## Overview

SocketDispatcher is a lightweight C# library for bi‑directional TCP socket communication using JSON‑encoded messages. It provides:

* **Dynamic payloads**: Messages carry a runtime-defined dictionary of key/value data.
* **Envelopes with callbacks**: Each message type is wrapped in an envelope that invokes a user‑supplied `Action<IMessage>` when received.
* **Type‑safe dispatching**: A central dispatcher routes incoming JSON to the correct envelope and invokes its callback.
* **Symmetry with Python**: By using plain JSON and a simple dictionary structure, you can mirror message classes in Python (e.g. `dataclasses` or dynamic types) and maintain identical communication semantics on both sides.

## Features

* **SocketManager**: Simplifies opening, sending, and receiving raw JSON over TCP.
* **IMessage & DynamicMessage**: Defines a minimal message interface and a concrete implementation carrying a `Dictionary<string, object>`.
* **IMessageEnvelope & MessageEnvelope**: Wraps any `IMessage` and holds an `Action<IMessage>` callback for received messages, plus built‑in JSON (de)serialization.
* **MessageDispatcher**: Registers callbacks by message name and automatically invokes the right handler for each incoming JSON.
* **Example Trading Envelopes**: Pre‑configured envelopes for `NewOrder`, `PriceUpdate`, and `AccountBalance` to kickstart typical trading scenarios.

## Getting Started

1. **Clone or download** this repository.
2. Create a new .NET Console project or integrate into your existing solution:

   ```bash
   dotnet new console -n SocketClient
   cd SocketClient
   ```
3. **Add** the `SocketDispatcher` source files to your project.
4. **Install** .NET 6.0+ SDK if you haven’t already.
5. **Build** and **run**:

   ```bash
   dotnet run
   ```

## Project Structure

```
SocketDispatcher/
├── IMessage.cs             # Base interface for dynamic payloads
├── DynamicMessage.cs       # Concrete implementation of IMessage
├── IMessageEnvelope.cs     # Interface for message envelopes with callbacks
├── MessageEnvelope.cs      # JSON‑aware envelope implementation
├── MessageDispatcher.cs    # Routes incoming JSON to registered handlers
├── TradingEnvelopes.cs     # Three example envelopes: NewOrder, PriceUpdate, AccountBalance
├── SocketManager.cs        # Low‑level TCP connect/send/receive utility
└── Program.cs              # Sample console app wiring everything together
```

## Usage Example (C#)

```csharp
// 1) Connect to server
env var manager = await SocketManager.ConnectAsync("localhost", 9000);

// 2) Set up dispatcher and register handlers
var dispatcher = new MessageDispatcher();
dispatcher.Register("NewOrder", msg => Console.WriteLine("Order received!"));

dispatcher.Register("PriceUpdate", msg => Console.WriteLine("Price updated!"));

dispatcher.Register("AccountBalance", msg => Console.WriteLine("Balance updated!"));

// 3) Send a NewOrder envelope
await manager.SendAsync(TradingEnvelopes.NewOrder.ToJson());

// 4) Enter receive loop
time while(true)
{
    var incomingJson = await manager.ReceiveAsync();
    dispatcher.Dispatch(incomingJson);
}
```

## Symmetry with Python

On the Python side, you can represent messages using simple dataclasses or even dynamic types:

```python
import json
from dataclasses import dataclass

@dataclass
class MessageEnvelope:
    Name: str
    Data: dict

# Deserialize incoming JSON:
env json_str = socket.recv(4096).decode()
env env = MessageEnvelope(**json.loads(json_str))
print(env.Name, env.Data)

# To send:
payload = {
    "Name": "NewOrder",
    "Data": { "OrderId": "...", "Symbol": "AAPL", "Side": "BUY", "Quantity": 100, "Price": 172.35 }
}
socket.send(json.dumps(payload).encode())
```

## Contributing

1. Fork the repository.
2. Create a feature branch (`git checkout -b feature/MyFeature`).
3. Commit your changes (`git commit -am 'Add new feature'`).
4. Push to the branch (`git push origin feature/MyFeature`).
5. Open a pull request.

## License

This project is released under the MIT License. See `LICENSE` for details.

## Packages

[![NuGet version (NeoNix-QtPy-Models)](https://img.shields.io/nuget/v/NomePacchetto.svg?style=flat-square)](https://www.nuget.org/packages/NeoNix-QtPy-Models/)
[![NuGet version (My.JsonSchema.Generator)](https://img.shields.io/nuget/v/NomePacchetto.svg?style=flat-square)](https://www.nuget.org/packages/My.JsonSchema.Generator/)
[![NuGet version (NeoNix-QtPy-Sdk-Serialization)](https://img.shields.io/nuget/v/NomePacchetto.svg?style=flat-square)](https://www.nuget.org/packages/NeoNix-QtPy-Sdk-Serialization/)
[![NuGet version (QunatCliKit)](https://img.shields.io/nuget/v/NomePacchetto.svg?style=flat-square)](https://www.nuget.org/packages/QunatCliKit/)

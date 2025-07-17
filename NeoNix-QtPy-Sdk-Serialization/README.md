# NeoNix-QtPy-Sdk-Serialization

A .NET 8 library for JSON‑based messaging over TCP and JSON Schema generation for IMessage types.

## Features

- **MessageHub**: Singleton for bi‑directional JSON messaging over TCP via `MessageHub.Init`, `SendAsync` and background receive loop :contentReference[oaicite:11]{index=11}.  
- **Dispatchable Handlers**: Register, override or unregister typed callbacks implementing `IDispatchable` (`Register`, `Override`, `UnRegiste`) to process incoming messages :contentReference[oaicite:12]{index=12}.  
- **SchemaManager**: Scan assemblies or the executing assembly, generate JSON Schemas for all concrete `IMessage` implementations, and write them to individual `.json` files :contentReference[oaicite:13]{index=13}.  
- **SocketManager**: Robust TCP wrapper with length‑prefixed JSON send/receive and timeout support :contentReference[oaicite:14]{index=14}.

## Getting Started

### Install from NuGet

```xml
<PackageReference Include="NeoNix-QtPy-Sdk-Serialization" Version="1.0.0" />
````

Then restore and build:

```bash
dotnet restore
dotnet build -c Release
```

### Basic Usage

```csharp
using NeoNix_QtPy_Sdk_Serialization;
using NeoNix_QtPy_Sdk_Serialization.Services;
using NeoNix_QtPy_Sdk_Serialization.Interfaces;

// 1) Initialize connection
await MessageHub.Instance.Init("host.example.com", 12345);

// 2) Register a handler for IMessage types
var inbox = new MyDispatchableHandler();   // implements IDispatchable
MessageHub.Instance.Register(inbox);

// 3) Send a message
await MessageHub.Instance.SendAsync(inbox);

// 4) Export JSON Schemas to disk
MessageHub.ExportSchemas("schemas");
```

### API Overview

* **`IMessage`**: Contract for payload types with a `Title` property .
* **`IDispatchable`**: Handler interface exposing `Name`, `Message`, `ToJson()`, and `Invoke()` .
* **`DispatchableBaseClass<T>`**: Base implementation with typed callback and `OnReceived` event .
* **`MessageDispatcher`**: Internal router that invokes registered `IDispatchable` by message name .
* **`SocketManager`**: Low‑level TCP socket helper with connect/send/receive and timeout logic .
* **`SchemaManager`**: Static class to generate and write JSON Schema files from `IMessage` types in assemblies .

## Contributing

1. Fork the repo
2. Create a branch: `git checkout -b feature/YourFeature`
3. Add tests and update docs
4. Open a Pull Request against `main`

Please follow existing code style and include unit tests where applicable.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.


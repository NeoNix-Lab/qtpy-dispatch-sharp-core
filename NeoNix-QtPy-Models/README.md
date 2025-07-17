````markdown
# QunatCliKit

A cross‑platform .NET 8 command‑line tool to manage the `QT_SDK_PATH` environment variable and generate JSON Schemas for types implementing `IMessage`.

## Features

- **Wizard**  
  Interactive setup for the `QT_SDK_PATH` environment variable (User scope).

- **Serialize**  
  Scans a folder of assemblies (DLLs), finds all implementations of `IMessage`, and exports JSON Schemas for them.

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)

## Installation

### As a Global Tool

```bash
dotnet tool install --global qunatclikit
````

### As a Local Tool (per‑project)

```bash
cd path/to/your/project
dotnet new tool-manifest --force
dotnet tool install --local --add-source <YOUR_NUGET_FEED_URL> qunatclikit
dotnet tool restore
```

## Usage

### Wizard

Launch the interactive wizard to set or override the `QT_SDK_PATH` environment variable:

```bash
qunatclikit wizard
```

### Serialize

Generate JSON Schemas for all `IMessage` implementations found in the specified assemblies folder:

```bash
qunatclikit serialize --assemblies "/path/to/your/assemblies"
```

## Build from Source

```bash
git clone https://github.com/yourorg/QunatCliKit.git
cd QunatCliKit
dotnet build -c Release
dotnet pack -c Release -o nupkg
```

To test locally:

```bash
dotnet tool install --global --add-source ./nupkg qunatclikit
```

## Contributing

1. Fork the repository
2. Create a branch: `git checkout -b feature/your-feature`
3. Add your changes, tests, and documentation
4. Submit a pull request against the `main` branch

Please follow the existing code style and include unit tests where appropriate.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

```
::contentReference[oaicite:0]{index=0}
```

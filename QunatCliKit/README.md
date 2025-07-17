````markdown
# QunatCliKit

A cross‑platform .NET 8 command‑line tool to manage the `QT_SDK_PATH` environment variable via an interactive wizard and to generate JSON Schemas for your `IMessage` types.

## Features

- **Interactive Wizard**  
  Guides you to set, get or exit the shared‐schema folder path. Uses Spectre.Console for a rich console UI and `ConsolePrompt` for consistent prompts :contentReference[oaicite:6]{index=6}.

- **Serialize Command**  
  Scans a folder of assemblies (DLLs), loads all implementations of `IMessage`, and exports JSON Schemas via `SchemaManager` :contentReference[oaicite:7]{index=7}.

- **Environment & Path Handling**  
  Validates, creates, and overrides the `QT_SDK_PATH` user‑scope environment variable using robust path checks :contentReference[oaicite:8]{index=8}.

- **Simple Command Configuration**  
  Builds a `RootCommand` and parser with `CommandConfigurator`, keeping configuration decoupled from handlers :contentReference[oaicite:9]{index=9}.

- **Smart Console Input**  
  Tokenizes user input to handle quoted arguments and supports `exit`/`q` shortcuts via `ConsolePrompt` :contentReference[oaicite:10]{index=10}.

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- (Optional) A NuGet feed containing the `qunatclikit` package

## Installation

### As a Global Tool

```bash
dotnet tool install --global qunatclikit
````

### As a Local Tool

```bash
cd path/to/your/project
dotnet new tool-manifest --force
dotnet tool install --local --add-source <YOUR_NUGET_FEED_URL> qunatclikit
dotnet tool restore
```

## Usage

### Wizard

Launch the interactive menu to set or display your shared schema folder path:

```bash
qunatclikit wizard
```

Options in the menu:

* 📁 Set shared schema folder path
* 📂 Get shared schema folder path
* 🚪 Exit

### Serialize

Generate JSON Schemas for all `IMessage` types found in a directory of DLLs:

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
2. Create a branch: `git checkout -b feature/YourFeature`
3. Implement your feature, add tests and update the README
4. Submit a Pull Request against `main`

Please follow the existing code style and include unit tests where applicable.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.\`\`\`

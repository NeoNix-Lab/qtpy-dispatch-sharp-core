# qt-py-sdk Path Manager

A .NET CLI tool to set and manage the `QT_SDK_PATH` environment variable for Neonix Qt-Python SDK projects.

## Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) or later

## Installation

### Global Tool

```bash
dotnet tool install --global qt-py-sdk

### Local Tool (per progetto)

```bash
cd path/to/your/project
dotnet new tool-manifest --force
dotnet tool install --local --add-source <your-feed-url> qt-py-sdk
```

## Usage

```bash
qt-py-sdk override
```

* **override**
  Verifica se la variabile `QT_SDK_PATH` esiste; se manca, apre una selezione guidata (CLI) e la imposta come variabile **User**.

## Commands

| Command    | Description                                                   |
| ---------- | ------------------------------------------------------------- |
| `override` | Esegue il wizard per impostare `QT_SDK_PATH` se non definita. |
| `--help`   | Mostra la guida ai comandi.                                   |

## Integration in MSBuild

Per richiamare automaticamente il tool in un progetto che dipende dalla libreria, aggiungi nel `.csproj`:

```xml
<PropertyGroup>
  <RestoreSources>$(RestoreSources);<your-nuget-feed-url></RestoreSources>
</PropertyGroup>

<Target Name="EnsureQtSdkPath"
        BeforeTargets="Build"
        Condition="'$(QT_SDK_PATH)' == ''">
  <Exec Command="dotnet tool run qt-py-sdk -- override" />
</Target>
```

## Contributing

1. Fork del repository
2. Crea un branch feature/my-feature
3. Aggiungi test e documentazione
4. PR verso il branch `main`

## License

This project is licensed under the MIT License – see the [LICENSE](Licens.txt) file for details.


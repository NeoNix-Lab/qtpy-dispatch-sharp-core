# Solution Structure

This repository contains a collection of small projects that work together to demonstrate a JSON based socket messaging architecture. The Visual Studio solution `socket_dispatcher_pseudo/socket_dispatcher_pseudo.sln` groups them into a single build.

```
.
├── Generator
│   ├── MyGeneratorSdk
│   └── nupkgs
├── IndicatorExample
│   └── Gen
├── NeoNix-QtPy-Models
├── NeoNix-QtPy-Sdk-Serialization
│   ├── Interfaces
│   ├── Services
│   ├── build
│   └── buildTransitive
├── QunatCliKit
│   ├── Commands
│   ├── Helpers
│   ├── Options
│   └── Services
├── SharedModels
├── SocketDispatcherPseudo.Tests
├── TestCodeGenerator
│   └── Schemas
├── socket_dispatcher_pseudo
│   ├── Interfaces
│   ├── Schemas
│   └── build
└── socket_dispatcher_python
```

## 1. Core Libraries

### `NeoNix-QtPy-Models`
Minimal interface library defining `IMessage` used across the solution.

### `NeoNix-QtPy-Sdk-Serialization`
Provides `MessageHub`, `SchemaManager` and `SocketManager` for sending and receiving JSON messages. Depends on `NeoNix-QtPy-Models`.

### `socket_dispatcher_pseudo`
Demonstrates message dispatch logic using `DynamicMessage`, `MessageEnvelope` and `MessageDispatcher`.

## 2. Utilities and Tooling

### `Generator`
Roslyn source generator that converts JSON Schema files into C# classes and is packaged as a NuGet analyzer.

### `QunatCliKit`
Command line tool (`qunatclikit`) to manage SDK paths and export JSON schemas. References `NeoNix-QtPy-Models`.

### `TestCodeGenerator`
Simple executable to test the generator.

## 3. Sample Models and Examples

### `SharedModels`
Example `IMessage` implementations used by other samples. References `NeoNix-QtPy-Models`.

### `IndicatorExample`
Quantower trading indicator example referencing `SharedModels` and `NeoNix-QtPy-Sdk-Serialization`.

### `socket_dispatcher_python`
Python client that mirrors the JSON length-prefixed protocol used in C#.

## 4. Tests

### `SocketDispatcherPseudo.Tests`
Unit tests (xUnit) exercising schema generation and dispatching. References `QunatCliKit`.

## Projects in the Solution

The `.sln` file declares the following projects:

```
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "SocketDispatcherPseudo.Tests", "..\SocketDispatcherPseudo.Tests\SocketDispatcherPseudo.Tests.csproj", "{22582769-6923-46F3-9E38-67F51E98BA93}"
Project("{888888A0-9F3D-457C-B088-3A5042F75D52}") = "socket_dispatcher_python", "..\socket_dispatcher_python\socket_dispatcher_python.pyproj", "{AC4D94FA-F3A1-4B1E-8331-4CAFA4183B6B}"
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Generator", "..\Generator\Generator.csproj", "{4393741F-A45C-486D-8D72-D5BB98C4D2E9}"
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "Solution Items", "Solution Items", "{168DAFC2-91A8-47CA-BEC8-C395BB45F1BF}"
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "NeoNix-QtPy-Sdk-Serialization", "..\NeoNix-QtPy-Sdk-Serialization\NeoNix-QtPy-Sdk-Serialization.csproj", "{66F48E48-C81E-4F4F-BAE4-C7D000D32CC5}"
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "QunatCliKit", "..\QunatCliKit\QunatCliKit.csproj", "{EF4AB179-A705-6A43-F488-E2B5C7CA78E1}"
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "IndicatorExample", "..\IndicatorExample\IndicatorExample.csproj", "{52DEDD99-102E-FDAF-FD94-AACEE36AE33D}"
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "NeoNix-QtPy-Models", "..\NeoNix-QtPy-Models\NeoNix-QtPy-Models.csproj", "{501089A9-0517-487E-A092-E270E21CF470}"
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "SharedModels", "..\SharedModels\SharedModels.csproj", "{65C538FD-0E6B-41E9-8313-3B19ED409B99}"
```

# Known Issues

This file lists outstanding TODO items and code quality issues discovered in the repository.

## TODO comments
- `NeoNix-QtPy-Sdk-Serialization/Interfaces/DispatchableBaseClass.cs`: convert generic type to `IMessage`.
- `NeoNix-QtPy-Sdk-Serialization/Services/SocketManager.cs`: fix accessibility of `SocketManager`.
- `QunatCliKit/Services/CommandConfigurator.cs`: remove duplicate root instance or implement singleton.
- `NeoNix-QtPy-Sdk-Serialization/TODO.cs`: multiple tasks including mapping title to type, completing `MessageHub` workflow, adding thread safety, async callbacks, serialization improvements, tests, MSBuild integration, documentation, NuGet packaging and schema export support.

## Python style
- `socket_dispatcher_python/socket_dispatcher_python.py` has long lines that violate flake8 `E501`.

## Test configuration
- `SocketDispatcherPseudo.Tests/SchemaManagerTests.cs` previously referenced an absolute path. It has been updated to use a path relative to the test directory.


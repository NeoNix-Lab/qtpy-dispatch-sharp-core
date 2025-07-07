// TODO: Map Title to Type
//   - Create mapping string->Type for all IMessage implementations
//   - Use in MessageHub.Dispatch and MessageDispatcher.Dispatch

// TODO: Complete MessageHub workflow
//   - In Init(): start read loop, parse JSON to IMessage, dispatch by Title
//   - Support SendAsync for IDispatchable and raw JSON
//   - Implement proper cancellation and error handling in _ReadLoop

// TODO: Add thread-safety to MessageHub and MessageDispatcher
//   - Use ConcurrentDictionary for _envelopes
//   - Lock or synchronize read/write of handlers

// TODO: Support async callbacks in DispatchableBaseClass
//   - Change Callback to Func<TMessage, Task>
//   - Make Invoke() return Task and await Callback

// TODO: Improve JSON serialization/deserialization
//   - Unify FromJson/ToJson in DispatchableBaseClass
//   - Ensure dynamic JSON parsing handles unknown fields

// TODO: Write Unit & Integration Tests
//   - Test MessageHub.Init, SendAsync, Dispatch scenarios
//   - Validate error cases: unregistered type, invalid JSON, timeout

// TODO: Add MSBuild integration for C# code generation (optional)
//   - Generate Typed IMessage classes from JSON schema at build time
//   - Include generated files in project

// TODO: Documentation & Samples
//   - Update README with new API changes (method names, async support)
//   - Provide code sample for Register, Dispatch, SendAsync
//   - Document thread-safety and cancellation usage

// TODO: NuGet packaging
//   - Exclude internal classes
//   - Include XML docs
//   - Validate package contents for public API surface

// TODO: Export user-defined IMessage schemas
//   - Discover all types implementing IMessage in target assemblies
//   - Use Newtonsoft.Json.Schema.JSchemaGenerator to generate JSON Schema
//   - Write schemas to configured output folder
//   - Provide user API to trigger export at build-time or runtime
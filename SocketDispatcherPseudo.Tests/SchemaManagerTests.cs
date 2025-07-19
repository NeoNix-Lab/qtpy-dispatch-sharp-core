using NeoNix_QtPy_Models;
using QunatCliKit.Services;
using System.Reflection;

namespace SocketDispatcherPseudo.Tests
{
    public class SchemaManagerTests
    {
        [Fact]
        public void ExportAll_GeneratesSchemas_FromHardcodedDll()
        {
            // Resolve the SharedModels assembly relative to the test directory
            var dllPath = Path.GetFullPath(
                Path.Combine(AppContext.BaseDirectory,
                             "../../../../SharedModels/bin/Debug/net8.0/SharedModels.dll"));

            var outputPath = Path.Combine(Path.GetTempPath(), "schemas_test");

            if (Directory.Exists(outputPath))
                Directory.Delete(outputPath, true);

            // 🧪 Load the assembly containing the test models
            var assembly = Assembly.LoadFrom(dllPath);
            List<string> Debugs = new List<string>();

            foreach (var t in assembly.GetTypes())
            {
                Debugs.Add($"[Debug] Type: {t.FullName}, Is IMessage: {typeof(IMessage).IsAssignableFrom(t)}");
            }

            // 🚀 Export schemas
            SchemaManager.ExportAll(outputPath, assembly);

            // ✅ Check result
            var generatedFiles = Directory.GetFiles(outputPath, "*.json");

            Assert.NotEmpty(generatedFiles); // At least one schema should be generated

            // (Optional) cleanup
            // Directory.Delete(outputPath, true);
        }
    }
}

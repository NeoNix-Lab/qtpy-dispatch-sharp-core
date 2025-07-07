using NeoNix_QtPy_Sdk_Serialization.Interfaces;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NeoNix_QtPy_Sdk_Serialization.Services
{
    public static class SchemaManager
    {
        private static List<string> _schemas = new List<string>();

        /// <summary>
        /// Discovers all non-abstract types implementing IMessage, generates their JSON Schema,
        /// and writes each schema to a separate .json file in <paramref name="outputDirectory"/>.
        /// </summary>
        /// <param name="outputDirectory">
        /// Path to the folder where schema files will be written. It will be created if missing.
        /// </param>
        /// <param name="assemblies">
        /// (Optional) Assemblies to scan. If null or empty, uses all loaded assemblies.
        /// </param>
        public static void ExportAll(
            string outputDirectory,
            params Assembly[] assemblies)
        {
            // Ensure output folder exists
            Directory.CreateDirectory(outputDirectory);

            SchemaManager.Load(outputDirectory);

            // Determine which assemblies to scan
            var toScan = (assemblies != null && assemblies.Length > 0)
                ? assemblies
                : AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => !a.IsDynamic)
                    .ToArray();

            // Find all concrete types implementing IMessage
            var messageTypes = toScan
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(IMessage).IsAssignableFrom(t)
                            && !t.IsInterface
                            && !t.IsAbstract
                            && !_schemas.Contains(t.Name));

            var generator = new JSchemaGenerator();

            foreach (var type in messageTypes)
            {
                // Generate the schema
                JSchema schema = generator.Generate(type);

                // Build output path: e.g. "./schemas/UserMessage.json"
                string fileName = Path.Combine(outputDirectory, $"{type.Name}.json");

                // Write out the schema
                File.WriteAllText(fileName, schema.ToString());

                Console.WriteLine($"[SchemaExporter] Generated schema for {type.Name} → {fileName}");
            }
        }

        private static void Load(string _outputDir)
        {
            _schemas.Clear();
            foreach (var file in Directory.GetFiles(_outputDir, "*.json"))
            {
                var txt = File.ReadAllText(file);
                var schema = JSchema.Parse(txt);
                // assume your schema has a Name or use filename
                var key = schema.Title ?? Path.GetFileNameWithoutExtension(file);
                _schemas.Add(key);
            }
        }
    }


    
}

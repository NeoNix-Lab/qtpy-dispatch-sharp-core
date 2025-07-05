using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;
using System;
using System.IO;

namespace Generator
{
    [Generator]
    public class JsonSchemaSourceGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // 1) Prendi tutti gli AdditionalFiles che finiscono con .json
            var schemaFiles = context.AdditionalTextsProvider
                                      .Where(at => at.Path.EndsWith(".json", StringComparison.OrdinalIgnoreCase));

            // 2) Per ciascuno, leggi il testo, parse JSON‐Schema e genera il C# code
            var generatedSchemas = schemaFiles.Select((at, ct) =>
            {
                // nome del modello = filename senza estensione
                var fileName = Path.GetFileNameWithoutExtension(at.Path);

                // contenuto JSON
                var json = at.GetText(ct)?.ToString() ?? "";
                // parse asincrono → blocchiamo sul risultato perché siamo in design‐time
                var schema = JsonSchema.FromJsonAsync(json, ct).ConfigureAwait(false).GetAwaiter().GetResult();

                // configura il generator C#
                var settings = new CSharpGeneratorSettings
                {
                    Namespace = "MyApp.Models",
                    ClassStyle = CSharpClassStyle.Poco,
                    GenerateDataAnnotations = true
                };
                var generator = new CSharpGenerator(schema, settings);

                // genera il file intero
                var code = generator.GenerateFile();

                return (FileName: fileName, Code: code);
            });

            // 3) Registra l’output: per ogni tupla (FileName, Code) crei una fonte
            context.RegisterSourceOutput(generatedSchemas, (spc, tuple) =>
            {
                // hintName = <ModelName>.g.cs
                var hintName = tuple.FileName + ".g.cs";
                spc.AddSource(hintName, SourceText.From(tuple.Code, Encoding.UTF8));
            });
        }
    }
}

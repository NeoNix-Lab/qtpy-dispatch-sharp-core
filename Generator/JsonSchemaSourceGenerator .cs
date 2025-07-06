using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;

namespace Generator
{
    [Generator]
    public class JsonSchemaSourceGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // 0) Post-init per forzare il caricamento del generator
            context.RegisterPostInitializationOutput(ctx =>
                ctx.AddSource("JsonSchemaGeneratorInit.g.cs",
                    SourceText.From("// JsonSchemaSourceGenerator initialized\n", Encoding.UTF8)));

            // 1) Filtro solo i .json dagli AdditionalFiles
            var schemaFiles = context.AdditionalTextsProvider
                                      .Where(at => at.Path.EndsWith(".json", StringComparison.OrdinalIgnoreCase));

            // 2) Per ogni .json, generiamo il C# code nel callback
            context.RegisterSourceOutput(schemaFiles, (spc, at) =>
            {
                var fileName = Path.GetFileNameWithoutExtension(at.Path);

                try
                {
                    // Legge il JSON
                    var json = at.GetText(spc.CancellationToken)?.ToString() ?? string.Empty;
                    // Parso asincrono bloccante
                    var schema = JsonSchema.FromJsonAsync(json, spc.CancellationToken)
                                           .GetAwaiter().GetResult();

                    //// Namespace personalizzabile via proprietà MSBuild
                    //var globalOpts = spc.AnalyzerConfigOptionsProvider.GlobalOptions;
                    //string ns = "MyApp.Models";
                    //if (globalOpts.TryGetValue("build_property.JsonSchemaNamespace", out var nsOption)
                    //    && !string.IsNullOrWhiteSpace(nsOption))
                    //{
                    //    ns = nsOption;
                    //}

                    // Impostazioni generator C#
                    var settings = new CSharpGeneratorSettings
                    {
                        Namespace = "Generated",
                        ClassStyle = CSharpClassStyle.Poco,
                        GenerateDataAnnotations = true
                    };

                    var generator = new CSharpGenerator(schema, settings);
                    var code = generator.GenerateFile();

                    // Aggiunge il file .g.cs
                    spc.AddSource(fileName + ".g.cs", SourceText.From(code, Encoding.UTF8));
                }
                catch (Exception ex)
                {
                    // Report diagnostica in caso di errore di parsing
                    var diag = Diagnostic.Create(
                        new DiagnosticDescriptor(
                            "JSGEN001",
                            "Errore parsing JSON-Schema",
                            "Fallito parsing di '{0}.json': {1}",
                            "JsonSchemaGenerator",
                            DiagnosticSeverity.Warning,
                            isEnabledByDefault: true),
                        Location.None,
                        fileName,
                        ex.Message);

                    spc.ReportDiagnostic(diag);
                }
            });
        }
    }
}

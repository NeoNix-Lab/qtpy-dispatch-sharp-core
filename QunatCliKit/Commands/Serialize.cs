using NeoNix_QtPy_Sdk_Serialization;
using NeoNix_QtPy_Sdk_Serialization.Services;
using QunatCliKit.Helpers;
using Spectre.Console;
using System.CommandLine;
using System.Reflection;

namespace QunatCliKit.Commands
{
    internal class Serialize
    {

        public static Command Create()
        {
            var cmd = new Command("serialize", "Serialize al Imessage Files") {
                new Option<string>(
                    "--assemblies",
                    description: "Cartella contenente le DLL da caricare"){IsRequired = true}

            };

            cmd.SetHandler((string assemblies) =>
            {
                var env_var = Environment.GetEnvironmentVariable("QT_SDK_PATH", EnvironmentVariableTarget.User);
                bool isValidPath = PathService.ValidatePath(env_var);

                if (!isValidPath)
                {
                    AnsiConsole.MarkupLine($"[red]Your path. {env_var}[/]");
                    AnsiConsole.MarkupLine("[red]Invalid path. Please set a valid Path.[/]");
                    var input = ConsolePrompt.Ask($"Sdk Wizard > press any to continue ");
                }

                if (isValidPath)
                {
                    AnsiConsole.MarkupLine($"[green]Your path. {env_var}[/]");
                    AnsiConsole.MarkupLine("[gren]Serialization start[/]");

                    try
                    {
                        SchemaManager.ExportAll(env_var, LoadAssembliesFrom(assemblies));
                        MessageHub.ExportSchemas(env_var);
                        AnsiConsole.MarkupLine("[green]Serialization done.[/]");

                    }
                    catch (Exception ex)
                    {
                        AnsiConsole.MarkupLine($"[red]Error during serialization: {ex.Message}[/]");
                        throw;
                    }


                    string end = ConsolePrompt.Ask($"Sdk Wizard > Sdk Ready .... press any to continue ... q to quit ");
                }
                
                
            },cmd.Options[0]);

            return cmd;
        }

        static Assembly[] LoadAssembliesFrom(string directory)
        {
            var existing = AppDomain.CurrentDomain.GetAssemblies()
                .Select(a => a.Location)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var loaded = new List<Assembly>();
            foreach (var file in Directory.GetFiles(directory, "*.dll"))
            {
                if (!existing.Contains(file))
                {
                    var asm = AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(file));
                    loaded.Add(asm);
                }
            }
            return loaded.ToArray();
        }
    }
}

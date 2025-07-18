using NeoNix_QtPy_Models;
using QunatCliKit.Helpers;
using QunatCliKit.Services;
using Spectre.Console;
using System.CommandLine;
using System.Reflection;

namespace QunatCliKit.Commands
{
    internal class Serialize
    {

        public static Command Create()
        {
            var assembliesOption = new Option<string>(
                "--assemblies",
                description: "Directory containing the DLLs to load")
            {
                IsRequired = true
            };

            var cmd = new Command("serialize", "Serialize al Imessage Files") {
                assembliesOption

            };

            cmd.SetHandler<string>(ExecuteSerialization, assembliesOption);

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
                    var asm = Assembly.LoadFrom(file);
                    loaded.Add(asm);
                }
            }
            return loaded.ToArray();
        }

        private static Task ExecuteSerialization(string assemblies)
        {
            var env_var = Environment.GetEnvironmentVariable("QT_SDK_PATH", EnvironmentVariableTarget.User);
            bool isValidPath = PathService.ValidatePath(env_var);

            if (!isValidPath)
            {
                AnsiConsole.MarkupLine($"[red]Your path. >>> {env_var} <<<[/]");
                AnsiConsole.MarkupLine("[red]Invalid path. Please set a valid Path.[/]");
                var input = ConsolePrompt.Ask($"Sdk Wizard > press any to continue  then Run wizard");
            }
            if (isValidPath)
            {
                AnsiConsole.MarkupLine($"[green]Your path. {env_var}[/]");
                AnsiConsole.MarkupLine("[green]Serialization start[/]");
                try
                {
                    SchemaManager.ExportAll(env_var, LoadAssembliesFrom(assemblies));
                    AnsiConsole.MarkupLine("[green]Serialization done.[/]");
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Error during serialization: {ex.Message}[/]");
                    throw;
                }
            }

            return Task.CompletedTask;
        }


    }
}
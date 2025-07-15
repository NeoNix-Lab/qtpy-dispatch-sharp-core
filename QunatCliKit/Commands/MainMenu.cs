using QunatCliKit.Helpers;
using Spectre.Console;
using System.CommandLine;

namespace QunatCliKit.Commands
{
    public static class MainMenu
    {

        public static Command Create()
        {
            var cmd = new Command("wizard", "Menage shared schemas folder path");

            cmd.SetHandler(() =>
            {
                var choise = RunInteractiveMenu();

                if (choise == "📁  Set shared schema folder path")
                {
                    ReactToOverride();
                }
                else if (choise == "📂  Get shared schema folder path")
                {
                    ReactToDisplay();
                }
                else if (choise == "🚪  Exit")
                {
                    ReactToExit();
                }
            });

            return cmd;
        }
        

        private static void ReactToExit()
        {
            AnsiConsole.MarkupLine("[red]Exiting...[/]");
            Environment.Exit(0);
        }

        private static void ReactToDisplay()
        {
            var env_var = Environment.GetEnvironmentVariable("QT_SDK_PATH", EnvironmentVariableTarget.User);
            Console.WriteLine(env_var);

            AnsiConsole.MarkupLine("[green]Your Pat from env Var :[/]");
            AnsiConsole.MarkupLine($"[green]{env_var}[/]");

            string end = ConsolePrompt.Ask($"Sdk Wizard > Press any to continue ... q to quit ");
        }

        private static void ReactToOverride()
        {
            var input = ConsolePrompt.Ask($"Sdk Wizard > write your path ");
            bool isValidPath = PathService.ValidatePath(input);
            while (!isValidPath)
            {
                AnsiConsole.MarkupLine("[red]Invalid path. Please try again.[/]");
                input = ConsolePrompt.Ask($"Sdk Wizard > write your path ");
                isValidPath = PathService.ValidatePath(input);
            }

            if (isValidPath)
            {
                AnsiConsole.MarkupLine("[green]Valid path. Will create if dosent exsist.[/]");
                string input_continue = ConsolePrompt.Ask($"Sdk Wizard > press any to continue ... ");
                PathService.Exist(input);
                AnsiConsole.MarkupLine("[green]Path ready.[/]");
                string input_override = ConsolePrompt.Ask($"Sdk Wizard > Enviroment var will be overrided .... press any to continue ... ");
                PathService.Override(input);

                string end = ConsolePrompt.Ask($"Sdk Wizard > Sdk Ready .... press any to continue ... q to quit ");
            }
        }

        private static string RunInteractiveMenu()
        {
            var options = new List<string>
            {
                "📁  Set shared schema folder path",
                "📂  Get shared schema folder path",
                "🚪  Exit"
            };
            AnsiConsole.Clear();
            AnsiConsole.Write(
                new FigletText("Neonix qt-py Sdk")
                    .Centered()
                    .Color(Color.Green));

            var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Shared_Schema Wizard")
                        .PageSize(8)
                        .AddChoices(options));

            return choice;
        }
    }
}

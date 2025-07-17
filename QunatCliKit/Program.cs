using QunatCliKit.Commands;
using QunatCliKit.Helpers;
using QunatCliKit.Services;
using System.CommandLine;


var root = CommandConfigurator.ConfigureRootCommand();

var wizardCommand = MainMenu.Create();
var serializationCommand = Serialize.Create();

root.AddCommand(wizardCommand);
root.AddCommand(serializationCommand);

// create parser
var parser = CommandConfigurator.ConfigureParser(root);
var result = parser.Parse(args);

if (args.Length > 0)
{
    await root.InvokeAsync(args);
}


string? initalInput = null /*= "wizard"*/;


while (true)
{
    var env_var = Environment.GetEnvironmentVariable("QT_SDK_PATH", EnvironmentVariableTarget.User);

    string input;
    switch (initalInput == null)
    {
        case false:
            input = initalInput;
            initalInput = null;
            break;

        case true:
            input = ConsolePrompt.Ask($"Sdk Wizard > your path : {env_var} run wizard to overrided");
            break;
    }
    if (string.IsNullOrWhiteSpace(input))
        continue;

    var arg = ConsolePrompt.SmartSplit(input);

    if (arg.Length == 0)
    {
        Console.WriteLine("No command provided.");
        arg = new string[] { "--help" };
    }

    await root.InvokeAsync(arg);
}

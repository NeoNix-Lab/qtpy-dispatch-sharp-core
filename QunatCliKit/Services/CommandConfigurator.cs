using QunatCliKit.Options;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;

namespace QunatCliKit.Services
{
    public static class CommandConfigurator
    {
        public static RootCommand ConfigureRootCommand()
        {
            var root = new RootCommand("Neonix qt-py Sdk Wizard");
            return root;
        }

        public static Parser ConfigureParser(RootCommand root)
        {

            //📝 TODO: [trovare la doppia istanza di root o usare un singleton con parametro di init]

            return new CommandLineBuilder(root)
                //.UseDefaults()
                .Build();
        }
    }
}

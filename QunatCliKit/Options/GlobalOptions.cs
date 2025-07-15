using System.CommandLine;

namespace QunatCliKit.Options
{
    public static class GlobalOptions
    {

        public static Option<string> shared_schemas_path { get; } = new Option<string>("--path", "Path to shared shemas folder");
        
    }
}

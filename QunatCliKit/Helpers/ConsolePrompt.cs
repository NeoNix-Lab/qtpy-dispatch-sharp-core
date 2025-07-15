using System.Text.RegularExpressions;

namespace QunatCliKit.Helpers
{
    public class ConsolePrompt
    {
        public static string Ask(string label)
        {
            Console.Write($"📝 {label}: ");
            var input = Console.ReadLine()?.Trim();

            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase) || input.Equals("q", StringComparison.OrdinalIgnoreCase))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("👋 Exit requested. Exiting...");
                Console.ResetColor();
                Environment.Exit(0); // chiude tutto elegantemente
            }

            return input!;
        }

        public static string[] SmartSplit(string input)
        {
            return Regex.Matches(input, @"[\""].+?[\""]|\S+")
                .Select(m => m.Value.Trim('"'))
                .ToArray();
        }
    }
}

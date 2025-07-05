using System.IO;

namespace TestCodeGenerator
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Hello World");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                Environment.Exit(1);
            }
        }
        
        //public static async Task Run()
        //{
        //    var schemaDir = "Schemas";
        //    var outputDir = "GeneratedModels";
        //    Directory.CreateDirectory(outputDir);

        //    foreach (var file in Directory.GetFiles(schemaDir, "*.json"))
        //    {
        //        var schema = await JsonSchema.FromFileAsync(file);
        //        var generator = new CSharpGenerator(schema, new CSharpGeneratorSettings
        //        {
        //            Namespace = "MyApp.Models",
        //            ClassStyle = CSharpClassStyle.Poco
        //        });
        //        var code = generator.GenerateFile();
        //        var csFile = Path.Combine(outputDir,
        //            Path.GetFileNameWithoutExtension(file) + ".cs");
        //        File.WriteAllText(csFile, code);
        //        Console.WriteLine($"Generated {csFile}");
        //    }
        //}
    }
}

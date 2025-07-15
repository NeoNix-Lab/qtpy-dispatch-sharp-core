using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using socket_dispatcher_pseudo.Interfaces;

namespace socket_dispatcher_pseudo
{
    public class SchemaManager
    {
        private readonly string _schemaFolder;
        private readonly Dictionary<string, JSchema> _schemas = new();

        public SchemaManager(string schemaFolder)
        {
            _schemaFolder = schemaFolder;
            //Reload();
            // (optionally hook up a FileSystemWatcher to call Reload() on changes)
        }

        public void Reload()
        {
            _schemas.Clear();
            foreach (var file in Directory.GetFiles(_schemaFolder, "*.json"))
            {
                var txt = File.ReadAllText(file);
                var schema = JSchema.Parse(txt);
                // assume your schema has a Name or use filename
                var key = schema.Title ?? Path.GetFileNameWithoutExtension(file);
                _schemas[key] = schema;
            }
        }

        public IMessage ParseAndValidate(string rawJson)
        {
            var env = JObject.Parse(rawJson);
            var name = env["name"]!.ToString();
            if (!_schemas.TryGetValue(name, out var schema))
                throw new KeyNotFoundException($"Schema for '{name}' not found");

            if (!env.IsValid(schema, out IList<string> errors))
                throw new JsonSchemaException($"Invalid message: {string.Join("; ", errors)}");

            // extract payload dict
            var payload = (JObject)env["payload"]!;
            var data = payload.ToObject<Dictionary<string, object>>()!;
            return new DynamicMessage(name, data);
        }

        public void GenerateSchema(Type type)
        {
            try
            {
                JSchemaGenerator generator = new JSchemaGenerator();
                JSchema schema = generator.Generate(type);
                string fileName = Path.Combine(_schemaFolder, $"{type.Name}.json");
                File.WriteAllText(fileName, schema.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message, ex);
            }
          
        }
    }

}

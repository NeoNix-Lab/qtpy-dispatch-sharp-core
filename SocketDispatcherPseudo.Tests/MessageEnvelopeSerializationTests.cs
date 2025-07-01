using socket_dispatcher_pseudo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SocketDispatcherPseudo.Tests
{
    public class MessageEnvelopeSerializationTests
    {
        [Fact]
        public void MessageEnvelope_ToJson_And_FromJson_Roundtrip()
        {
            var payload = new Dictionary<string, object>
            {
                ["Symbol"] = "TSLA",
                ["Price"] = 630.5
            };
            var env = MessageEnvelope.Create("PriceUpdate", payload, _ => { });
            var json = env.ToJson();

            // Deserialize
            var roundtripped = MessageEnvelope.FromJson(json);
            // Data dovrebbe essere un IDictionary<string, object> o simile
            var data = roundtripped.Message.Data;
            Assert.IsAssignableFrom<IDictionary<string, object>>(data);

            var dict = (IDictionary<string, object>)data;
            Assert.True(dict.ContainsKey("Symbol"));
            Assert.True(dict.ContainsKey("Price"));

            Assert.IsType<string>(dict["Symbol"]);
            Assert.Equal("TSLA", (string)dict["Symbol"]);

            Assert.IsType<double>(dict["Price"]);
            Assert.Equal(630.5, (double)dict["Price"]);
        }
    }
}

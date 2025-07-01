using socket_dispatcher_pseudo;
using System.Text.Json;

namespace SocketDispatcherPseudo.Tests
{
    public class MessageDispatcherTests
    {
        [Fact]
        public void Dispatcher_ShouldIgnoreUnknownMessage()
        {
            var dispatcher = new MessageDispatcher();
            // no registration
            var jsonEnv = JsonSerializer.Serialize(new { Name = "Unknown", Data = new { } });
            // Should not throw
            dispatcher.Dispatch(jsonEnv);
        }
    }
}

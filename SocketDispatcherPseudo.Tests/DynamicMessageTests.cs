using socket_dispatcher_pseudo;

namespace SocketDispatcherPseudo.Tests
{
    public class DynamicMessageTests
    {
        [Fact]
        public void DynamicMessage_ShouldExposeNameAndData()
        {
            var data = new Dictionary<string, object> { ["Foo"] = 42 };
            var msg = new DynamicMessage("TestMessage", data);

            Assert.Equal("TestMessage", msg.Name);
            Assert.Same(data, msg.Data);
            Assert.True(msg.Data.ContainsKey("Foo"));
            Assert.Equal(42, msg.Data["Foo"]);
        }
    }
}
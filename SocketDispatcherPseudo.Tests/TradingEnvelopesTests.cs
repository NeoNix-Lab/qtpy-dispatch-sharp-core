using socket_dispatcher_pseudo;

namespace SocketDispatcherPseudo.Tests
{
    public class TradingEnvelopesTests
    {
        [Fact]
        public void NewOrderEnvelope_ShouldHaveExpectedFields()
        {
            var env = TradingEnvelopes.NewOrder;
            var data = env.Message.Data;

            Assert.Equal("NewOrder", env.Name);
            Assert.True(Guid.TryParse(data["OrderId"].ToString(), out _));
            Assert.Equal("AAPL", data["Symbol"]);
            Assert.Equal("BUY", data["Side"]);
            Assert.Equal(100, Convert.ToInt32(data["Quantity"]));
            Assert.Equal(172.35, Convert.ToDouble(data["Price"]));
        }

        [Fact]
        public void PriceUpdateEnvelope_ShouldContainBidAsk()
        {
            var env = TradingEnvelopes.PriceUpdate;
            var data = env.Message.Data;

            Assert.Equal("PriceUpdate", env.Name);
            Assert.True(data.ContainsKey("Bid"));
            Assert.True(data.ContainsKey("Ask"));
            Assert.IsType<DateTime>(data["Timestamp"]);
        }

        [Fact]
        public void AccountBalanceEnvelope_ShouldContainPositionsDictionary()
        {
            var env = TradingEnvelopes.AccountBalance;
            var data = env.Message.Data;

            Assert.Equal("AccountBalance", env.Name);
            Assert.True(data.ContainsKey("Positions"));
            var positions = data["Positions"] as Dictionary<string, object>;
            Assert.NotNull(positions);
            Assert.Equal(150, Convert.ToInt32(positions["AAPL"]));
            Assert.Equal(75, Convert.ToInt32(positions["MSFT"]));
        }
    }
}

namespace Nacos.Tests
{
    using System.Threading.Tasks;
    using Xunit;

    public class ConfigTest : TestBase
    {
        [Fact]
        public async Task GetConfig_Should_Succeed()
        {
            var request = new GetConfigRequest
            {
                DataId = "dataId",
                Group = "DEFAULT_GROUP",
                //Tenant = "tenant"
            };

            var res = await _client.GetConfigAsync(request);
            Assert.NotNull(res);
            Assert.Equal("test", res);
        }

        [Fact]
        public async Task PublishConfig_Should_Succeed()
        {
            var request = new PublishConfigRequest
            {
                DataId = "dataId",
                Group = "DEFAULT_GROUP",
                //Tenant = "tenant",
                Content = "test"
            };

            var res = await _client.PublishConfigAsync(request);
            Assert.True(res);
        }

        [Fact]
        public async Task RemoveConfig_Should_Succeed()
        {
            var request = new RemoveConfigRequest
            {
                DataId = "dataId",
                Group = "DEFAULT_GROUP",
                //Tenant = "tenant"
            };

            var res = await _client.RemoveConfigAsync(request);
            Assert.True(res);
        }

        [Fact]
        public async Task ListenerConfig_Should_Succeed()
        {
            var request = new ListenerConfigRequest
            {
                DataId = "dataId",
                //Group = "DEFAULT_GROUP",
                //Tenant = "tenant"
            };

            await _client.ListenerConfigAsync(request);

            Assert.True(true);

            await Task.Delay(50000);

            
        }
    }
}

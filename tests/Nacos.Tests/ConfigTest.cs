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
                Group = "group",
                Tenant = "tenant"
            };

            var res = await _client.GetConfigAsync(request);
            Assert.NotNull(res);
        }

        [Fact]
        public async Task PublishConfig_Should_Succeed()
        {
            var request = new PublishConfigRequest
            {
                DataId = "dataId",
                Group = "group",
                Tenant = "tenant"
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
                Group = "group",
                Tenant = "tenant"
            };

            var res = await _client.RemoveConfigAsync(request);
            Assert.True(res);
        }
    }
}

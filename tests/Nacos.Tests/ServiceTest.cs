namespace Nacos.Tests
{
    using System.Threading.Tasks;
    using Xunit;

    public class ServiceTest : TestBase
    {
        [Fact]
        public async Task CreateService_Should_Succeed()
        {
            var request = new CreateServiceRequest
            {
                ServiceName = "testservice"
            };

            var res = await _client.CreateServiceAsync(request);
            Assert.True(res);
        }

        [Fact]
        public async Task RemoveService_Should_Succeed()
        {
            var request = new RemoveServiceRequest
            {
                ServiceName = "testservice"
            };

            var res = await _client.RemoveServiceAsync(request);
            Assert.True(res);
        }

        [Fact]
        public async Task ModifyService_Should_Succeed()
        {
            var request = new ModifyServiceRequest
            {
                ServiceName = "testservice",
                ProtectThreshold = 0.5,
            };

            var res = await _client.ModifyServiceAsync(request);
            Assert.True(res);
        }

        [Fact]
        public async Task GetService_Should_Succeed()
        {
            var request = new GetServiceRequest
            {
                ServiceName = "testservice",
            };

            var res = await _client.GetServiceAsync(request);
            Assert.NotNull(res);
        }

        [Fact]
        public async Task ListServices_Should_Succeed()
        {
            var request = new ListServicesRequest
            {
                PageNo = 1,
                PageSize = 2,
            };

            var res = await _client.ListServicesAsync(request);
            Assert.NotNull(res);
        }
    }
}

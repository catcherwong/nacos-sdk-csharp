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

            };

            var res = await _client.CreateServiceAsync(request);
            Assert.True(res);
        }

        [Fact]
        public async Task RemoveService_Should_Succeed()
        {
            var request = new RemoveServiceRequest
            {

            };

            var res = await _client.RemoveServiceAsync(request);
            Assert.True(res);
        }

        [Fact]
        public async Task ModifyService_Should_Succeed()
        {
            var request = new ModifyServiceRequest
            {

            };

            var res = await _client.ModifyServiceAsync(request);
            Assert.True(res);
        }

        [Fact]
        public async Task GetService_Should_Succeed()
        {
            var request = new GetServiceRequest
            {

            };

            var res = await _client.GetServiceAsync(request);
            Assert.NotNull(res);
        }

        [Fact]
        public async Task ListServices_Should_Succeed()
        {
            var request = new ListServicesRequest
            {

            };

            var res = await _client.ListServicesAsync(request);
            Assert.NotNull(res);
        }
    }
}

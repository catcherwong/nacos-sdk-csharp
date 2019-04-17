namespace Nacos.Tests
{
    using System.Threading.Tasks;
    using Xunit;

    public class InstanceTest : TestBase
    {
        [Fact]
        public async Task RegisterInstance_Should_Succeed()
        {
            var request = new RegisterInstanceRequest
            {
                ServiceName = "aspnet",
                Ip = "127.0.0.1",
                Port = 5000
            };

            var res = await _client.RegisterInstanceAsync(request);
            Assert.True(res);
        }

        [Fact]
        public async Task RemoveInstance_Should_Succeed()
        {
            var request = new RemoveInstanceRequest
            {
                ServiceName = "aspnet",
                Ip = "127.0.0.1",
                Port = 5000
            };

            var res = await _client.RemoveInstanceAsync(request);
            Assert.True(res);
        }

        [Fact]
        public async Task ModifyInstance_Should_Succeed()
        {
            var request = new ModifyInstanceRequest
            {
                ServiceName = "aspnet",
                Ip = "127.0.0.1",
                Port = 5000
            };

            var res = await _client.ModifyInstanceAsync(request);
            Assert.True(res);
        }

        [Fact]
        public async Task ListInstances_Should_Succeed()
        {
            var request = new ListInstancesRequest
            {
                ServiceName = "aspnet",
            };

            var res = await _client.ListInstancesAsync(request);
            Assert.NotNull(res);
        }

        [Fact]
        public async Task GetInstance_Should_Succeed()
        {
            var request = new GetInstanceRequest
            {
                ServiceName = "aspnet",
                Ip = "127.0.0.1",
                Port = 5000
            };

            var res = await _client.GetInstanceAsync(request);
            Assert.NotNull(res);
        }

        [Fact]
        public async Task SendHeartbeat_Should_Succeed()
        {
            var request = new SendHeartbeatRequest
            {
                ServiceName = "aspnet",
                Beat = ""
            };

            var res = await _client.SendHeartbeatAsync(request);
            Assert.True(res);
        }

        [Fact]
        public async Task ModifyInstanceHealthStatus_Should_Succeed()
        {
            var request = new ModifyInstanceHealthStatusRequest
            {
                Ip = "127.0.0.1",
                Port = 5000,
                ServiceName = "aspnet",
                Healthy = true,
            };

            var res = await _client.ModifyInstanceHealthStatusAsync(request);
            Assert.True(res);
        }
    }
}

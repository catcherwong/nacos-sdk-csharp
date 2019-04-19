﻿namespace Nacos.Tests
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
                ServiceName = "testservice",
                Ip = "192.168.0.74",
                Port = 9999
            };

            var res = await _namingClient.RegisterInstanceAsync(request);
            Assert.True(res);
        }

        [Fact]
        public async Task RemoveInstance_Should_Succeed()
        {
            var request = new RemoveInstanceRequest
            {
                ServiceName = "testservice",
                Ip = "192.168.0.74",
                Port = 9999
            };

            var res = await _namingClient.RemoveInstanceAsync(request);
            Assert.True(res);
        }

        [Fact]
        public async Task ModifyInstance_Should_Succeed()
        {
            var request = new ModifyInstanceRequest
            {
                ServiceName = "testservice",
                Ip = "192.168.0.74",
                Port = 5000
            };

            var res = await _namingClient.ModifyInstanceAsync(request);
            Assert.True(res);
        }

        [Fact]
        public async Task ListInstances_Should_Succeed()
        {
            var request = new ListInstancesRequest
            {
                ServiceName = "testservice",
            };

            var res = await _namingClient.ListInstancesAsync(request);
            Assert.NotNull(res);
        }

        [Fact]
        public async Task GetInstance_Should_Succeed()
        {
            var request = new GetInstanceRequest
            {
                ServiceName = "testservice",
                Ip = "192.168.0.74",
                Port = 9999,                 
            };

            var res = await _namingClient.GetInstanceAsync(request);
            Assert.NotNull(res);
        }

        [Fact]
        public async Task SendHeartbeat_Should_Succeed()
        {
            var request = new SendHeartbeatRequest
            {
                ServiceName = "testservice",
                BeatInfo = new BeatInfo
                {
                    ServiceName = "testservice",
                    Ip = "192.168.0.74",
                    Port = 9999,                     
                }
            };

            var res = await _namingClient.SendHeartbeatAsync(request);
            Assert.True(res);
        }

        [Fact]
        public async Task ModifyInstanceHealthStatus_Should_Succeed()
        {
            var request = new ModifyInstanceHealthStatusRequest
            {
                Ip = "192.168.0.74",
                Port = 9999,
                ServiceName = "testservice",
                Healthy = false,
            };

            var res = await _namingClient.ModifyInstanceHealthStatusAsync(request);
            // 集群配置了健康检查时,该接口会返回错误
            Assert.False(res);
        }
    }
}

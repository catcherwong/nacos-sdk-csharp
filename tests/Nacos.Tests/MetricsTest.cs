namespace Nacos.Tests
{
    using System.Threading.Tasks;
    using Xunit;

    public class MetricsTest : TestBase
    {
        [Fact]
        public async Task GetMetrics_Should_Succeed()
        {          
            var res = await _client.GetMetricsAsync();
            Assert.NotNull(res);
        }
    }
}

namespace Nacos.Tests
{
    using System.Threading.Tasks;
    using Xunit;

    public class SwitchesTest : TestBase
    {
        [Fact]
        public async Task GetSwitches_Should_Succeed()
        {
            var res = await _client.GetSwitchesAsync();
            Assert.NotNull(res);
        }

        [Fact]
        public async Task ModifySwitches_Should_Succeed()
        {
            var request = new ModifySwitchesRequest
            {

            };

            var res = await _client.ModifySwitchesAsync(request);
            Assert.True(res);
        }
    }
}

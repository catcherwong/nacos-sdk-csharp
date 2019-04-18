namespace Nacos
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Nacos.Utilities;

    public partial class NacosClient : INacosClient
    {
        public async Task<GetMetricsResult> GetMetricsAsync()
        {
            var responseMessage = await DoRequestAsync(HttpMethod.Get, $"{_options.EndPoint}/nacos/v1/ns/operator/metrics");
            responseMessage.EnsureSuccessStatusCode();

            var result = await responseMessage.Content.ReadAsStringAsync();
            var obj = result.ToObj<GetMetricsResult>();
            return obj;
        }
    }
}

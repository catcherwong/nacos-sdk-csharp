namespace Nacos
{
    using Nacos.Exceptions;
    using Nacos.Utilities;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    public partial class NacosClient : INacosClient
    {
        public async Task<GetSwitchesResult> GetSwitchesAsync()
        {
            var responseMessage = await DoRequestAsync(HttpMethod.Get, "/nacos/v1/ns/operator/switches");
            responseMessage.EnsureSuccessStatusCode();

            var result = await responseMessage.Content.ReadAsStringAsync();
            var obj = result.ToObj<GetSwitchesResult>();
            return obj;
        }

        public async Task<bool> ModifySwitchesAsync(ModifySwitchesRequest request)
        {
            if (!request.IsValid())
            {
                throw new RequestInValidException("request 参数不合法");
            }

            var responseMessage = await DoRequestAsync(HttpMethod.Put, "/nacos/v1/ns/operator/switches", request.ToQueryString());
            responseMessage.EnsureSuccessStatusCode();

            var result = await responseMessage.Content.ReadAsStringAsync();
            return result.Equals("ok", StringComparison.OrdinalIgnoreCase);
        }
    }
}

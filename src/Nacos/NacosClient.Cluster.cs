namespace Nacos
{
    using Nacos.Exceptions;
    using Nacos.Utilities;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    public partial class NacosClient : INacosClient
    {
        public async Task<ListClusterServersResult> ListClusterServersAsync(ListClusterServersRequest request)
        {
            if (!request.IsValid())
            {
                throw new RequestInValidException("request 参数不合法");
            }

            var responseMessage = await DoRequestAsync(HttpMethod.Get, "/nacos/v1/ns/operator/servers", request.ToQueryString());
            responseMessage.EnsureSuccessStatusCode();

            var result = await responseMessage.Content.ReadAsStringAsync();
            var obj = result.ToObj<ListClusterServersResult>();
            return obj;
        }

        public async Task<GetCurrentClusterLeaderResult> GetCurrentClusterLeaderAsync()
        {
            var responseMessage = await DoRequestAsync(HttpMethod.Get, "/nacos/v1/ns/raft/leader");
            responseMessage.EnsureSuccessStatusCode();

            var result = await responseMessage.Content.ReadAsStringAsync();
            var obj = result.ToObj<GetCurrentClusterLeaderResult>();
            return obj;
        }
    }
}

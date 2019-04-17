namespace Nacos
{
    using Nacos.Exceptions;
    using Nacos.Utilities;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    public partial class NacosClient : INacosClient
    {
        public async Task<bool> RegisterInstanceAsync(RegisterInstanceRequest request)
        {
            if (!request.IsValid())
            {
                throw new RequestInValidException("request 参数不合法");
            }

            var responseMessage = await DoRequestAsync(HttpMethod.Post, "/nacos/v1/ns/instance", request.ToQueryString());
            responseMessage.EnsureSuccessStatusCode();

            var result = await responseMessage.Content.ReadAsStringAsync();
            return result.Equals("ok", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<bool> RemoveInstanceAsync(RemoveInstanceRequest request)
        {
            if (!request.IsValid())
            {
                throw new RequestInValidException("request 参数不合法");
            }

            var responseMessage = await DoRequestAsync(HttpMethod.Delete, "/nacos/v1/ns/instance", request.ToQueryString());
            responseMessage.EnsureSuccessStatusCode();

            var result = await responseMessage.Content.ReadAsStringAsync();
            return result.Equals("ok", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<bool> ModifyInstanceAsync(ModifyInstanceRequest request)
        {
            if (!request.IsValid())
            {
                throw new RequestInValidException("request 参数不合法");
            }

            var responseMessage = await DoRequestAsync(HttpMethod.Put, "/nacos/v1/ns/instance", request.ToQueryString());
            responseMessage.EnsureSuccessStatusCode();

            var result = await responseMessage.Content.ReadAsStringAsync();
            return result.Equals("ok", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<ListInstancesResult> ListInstancesAsync(ListInstancesRequest request)
        {
            if (!request.IsValid())
            {
                throw new RequestInValidException("request 参数不合法");
            }

            var responseMessage = await DoRequestAsync(HttpMethod.Get, "/nacos/v1/ns/instance/list", request.ToQueryString());
            responseMessage.EnsureSuccessStatusCode();

            var result = await responseMessage.Content.ReadAsStringAsync();
            var obj = result.ToObj<ListInstancesResult>();
            return obj;
        }

        public async Task<GetInstanceResult> GetInstanceAsync(GetInstanceRequest request)
        {
            if (!request.IsValid())
            {
                throw new RequestInValidException("request 参数不合法");
            }

            var responseMessage = await DoRequestAsync(HttpMethod.Get, "/nacos/v1/ns/instance", request.ToQueryString());
            responseMessage.EnsureSuccessStatusCode();

            var result = await responseMessage.Content.ReadAsStringAsync();
            var obj = result.ToObj<GetInstanceResult>();
            return obj;
        }

        public async Task<bool> SendHeartbeatAsync(SendHeartbeatRequest request)
        {
            if (!request.IsValid())
            {
                throw new RequestInValidException("request 参数不合法");
            }

            var responseMessage = await DoRequestAsync(HttpMethod.Put, "/nacos/v1/ns/instance/beat", request.ToQueryString());
            responseMessage.EnsureSuccessStatusCode();

            var result = await responseMessage.Content.ReadAsStringAsync();
            return result.Equals("ok", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<bool> ModifyInstanceHealthStatusAsync(ModifyInstanceHealthStatusRequest request)
        {
            if (!request.IsValid())
            {
                throw new RequestInValidException("request 参数不合法");
            }

            var responseMessage = await DoRequestAsync(HttpMethod.Put, "/nacos/v1/ns/health/instance", request.ToQueryString());
            responseMessage.EnsureSuccessStatusCode();

            var result = await responseMessage.Content.ReadAsStringAsync();
            return result.Equals("ok", StringComparison.OrdinalIgnoreCase);
        }
    }
}

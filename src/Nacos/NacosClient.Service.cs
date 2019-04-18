namespace Nacos
{
    using Nacos.Exceptions;
    using Nacos.Utilities;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    public partial class NacosClient : INacosClient
    {
        public async Task<bool> CreateServiceAsync(CreateServiceRequest request)
        {
            if (!request.IsValid())
            {
                throw new RequestInValidException("request 参数不合法");
            }

            var responseMessage = await DoRequestAsync(HttpMethod.Post, $"{_options.EndPoint}/nacos/v1/ns/service", request.ToQueryString());
            responseMessage.EnsureSuccessStatusCode();

            var result = await responseMessage.Content.ReadAsStringAsync();
            return result.Equals("ok", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<bool> RemoveServiceAsync(RemoveServiceRequest request)
        {
            if (!request.IsValid())
            {
                throw new RequestInValidException("request 参数不合法");
            }

            var responseMessage = await DoRequestAsync(HttpMethod.Delete, $"{_options.EndPoint}/nacos/v1/ns/service", request.ToQueryString());
            responseMessage.EnsureSuccessStatusCode();

            var result = await responseMessage.Content.ReadAsStringAsync();
            return result.Equals("ok", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<bool> ModifyServiceAsync(ModifyServiceRequest request)
        {
            if (!request.IsValid())
            {
                throw new RequestInValidException("request 参数不合法");
            }

            var responseMessage = await DoRequestAsync(HttpMethod.Put, $"{_options.EndPoint}/nacos/v1/ns/service", request.ToQueryString());
            responseMessage.EnsureSuccessStatusCode();

            var result = await responseMessage.Content.ReadAsStringAsync();
            return result.Equals("ok", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<GetServiceResult> GetServiceAsync(GetServiceRequest request)
        {
            if (!request.IsValid())
            {
                throw new RequestInValidException("request 参数不合法");
            }

            var responseMessage = await DoRequestAsync(HttpMethod.Get, $"{_options.EndPoint}/nacos/v1/ns/service", request.ToQueryString());
            responseMessage.EnsureSuccessStatusCode();

            var result = await responseMessage.Content.ReadAsStringAsync();
            var obj = result.ToObj<GetServiceResult>();
            return obj;
        }

        public async Task<ListServicesResult> ListServicesAsync(ListServicesRequest request)
        {
            if (!request.IsValid())
            {
                throw new RequestInValidException("request 参数不合法");
            }

            var responseMessage = await DoRequestAsync(HttpMethod.Get, $"{_options.EndPoint}/nacos/v1/ns/service/list", request.ToQueryString());
            responseMessage.EnsureSuccessStatusCode();

            var result = await responseMessage.Content.ReadAsStringAsync();
            var obj = result.ToObj<ListServicesResult>();
            return obj;
        }
    }
}

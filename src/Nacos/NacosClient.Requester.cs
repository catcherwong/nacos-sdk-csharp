namespace Nacos
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    public partial class NacosClient : INacosClient
    {    
        private async Task<HttpResponseMessage> DoRequestAsync(HttpMethod method, string url, string param = "")
        {
            var client = _clientFactory.CreateClient(ConstValue.ClientName);
            client.Timeout = TimeSpan.FromSeconds(_options.DefaultTimeOut);

            var requestUrl = string.IsNullOrWhiteSpace(param) ? url : $"{url}?{param}";

            var requestMessage = new HttpRequestMessage(method, requestUrl);
            var responseMessage = await client.SendAsync(requestMessage);
            return responseMessage;
        }
    }
}

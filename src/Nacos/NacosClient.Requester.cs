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

            var requestMessage = new HttpRequestMessage(method, $"{url}?{param}");
            var responseMessage = await client.SendAsync(requestMessage);
            return responseMessage;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nacos
{
    internal class NacosHttpClient
    {
        internal readonly HttpClientHandler HttpHandler;

        internal readonly HttpClient HttpClient;

        public NacosHttpClient()
        {
            HttpHandler = new HttpClientHandler();
            HttpClient = new HttpClient(HttpHandler);
            HttpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<HttpResponseMessage> DoRequestAsync(HttpMethod method, string url, string param = "", int timeOut = 8, Dictionary<string, string> headers = null)
        {
            var requestUrl = string.IsNullOrWhiteSpace(param) ? url : $"{url}?{param}";

            var requestMessage = new HttpRequestMessage(method, requestUrl);

            BuildHeader(requestMessage, headers);

            var responseMessage = await HttpClient.SendAsync(requestMessage);
            return responseMessage;
        }

        private void BuildHeader(HttpRequestMessage requestMessage, Dictionary<string, string> headers)
        {
            requestMessage.Headers.Clear();

            if (headers != null)
            {
                foreach (var item in headers)
                {
                    requestMessage.Headers.Add(item.Key, item.Value);
                }
            }

            requestMessage.Headers.Add("Client-Version", ConstValue.ClientVersion);
            requestMessage.Headers.Add("User-Agent", ConstValue.ClientVersion);
            requestMessage.Headers.Add("RequestId", Guid.NewGuid().ToString());
            requestMessage.Headers.Add("Request-Module", ConstValue.RequestModule);
        }
    }
}

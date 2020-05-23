using Nacos.Utilities;
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

        public async Task<HttpResponseMessage> DoRequestAsync(HttpMethod method, string url, string param = "", int timeOut = 8, Dictionary<string, string> headers = null, string secretKey = "")
        {
            var requestUrl = string.IsNullOrWhiteSpace(param) ? url : $"{url}?{param}";

            var requestMessage = new HttpRequestMessage(method, requestUrl);

            HttpClientFactoryUtil.BuildHeader(requestMessage, headers);
            if (!string.IsNullOrWhiteSpace(secretKey)) HttpClientFactoryUtil.BuildSignHeader(requestMessage, param, secretKey);

            var responseMessage = await HttpClient.SendAsync(requestMessage);
            return responseMessage;
        }
    }
}

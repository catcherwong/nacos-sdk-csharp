namespace Nacos.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    public static class HttpClientFactoryUtil
    {
        public static async Task<HttpResponseMessage> DoRequestAsync(this IHttpClientFactory factory, HttpMethod method, string url, string param = "", int timeOut = 8, Dictionary<string, string> headers = null)
        {
            var client = factory.CreateClient(ConstValue.ClientName);
            client.Timeout = TimeSpan.FromSeconds(timeOut);

            var requestUrl = string.IsNullOrWhiteSpace(param) ? url : $"{url}?{param}";

            var requestMessage = new HttpRequestMessage(method, requestUrl);

            BuildHeader(requestMessage, headers);

            var responseMessage = await client.SendAsync(requestMessage);
            return responseMessage;
        }

        private static void BuildHeader(HttpRequestMessage requestMessage, Dictionary<string, string> headers)
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

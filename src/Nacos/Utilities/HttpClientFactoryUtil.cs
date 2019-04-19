namespace Nacos.Utilities
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    public static class HttpClientFactoryUtil
    {
        public static async Task<HttpResponseMessage> DoRequestAsync(this IHttpClientFactory factory, HttpMethod method, string url, string param = "", int timeOut = 8)
        {
            var client = factory.CreateClient(ConstValue.ClientName);
            client.Timeout = TimeSpan.FromSeconds(timeOut);

            var requestUrl = string.IsNullOrWhiteSpace(param) ? url : $"{url}?{param}";

            var requestMessage = new HttpRequestMessage(method, requestUrl);
            var responseMessage = await client.SendAsync(requestMessage);
            return responseMessage;
        }
    }
}

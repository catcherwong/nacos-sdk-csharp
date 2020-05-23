namespace Nacos.Microsoft.Extensions.Configuration
{
    using Nacos.Config.Http;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class MsConfigServerHttpAgent : HttpAgent
    {
        private readonly NacosOptions _options;

        public MsConfigServerHttpAgent(NacosOptions options)
        {
            _options = options;
        }

        public override async Task<HttpResponseMessage> ReqApiAsync(HttpMethod httpMethod, string path, Dictionary<string, string> headers, Dictionary<string, string> paramValues, int timeout)
        {
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(timeout);

                var requestMessage = new HttpRequestMessage
                {
                    Method = httpMethod
                };

                var requestUrl = path;

                if (paramValues != null && paramValues.Any())
                {
                    if (httpMethod == HttpMethod.Post)
                    {
                        requestMessage.RequestUri = new Uri(requestUrl);
                        requestMessage.Content = new FormUrlEncodedContent(paramValues);
                    }
                    else
                    {
                        var query = HttpAgentCommon.BuildQueryString(paramValues);
                        requestMessage.RequestUri = new Uri($"{path}?{query}");
                    }
                }

                HttpAgentCommon.BuildHeader(requestMessage, headers);
                HttpAgentCommon.BuildSpasHeaders(requestMessage, paramValues, _options.AccessKey, _options.SecretKey);

                var responseMessage = await client.SendAsync(requestMessage);
                return responseMessage;
            }
        }
    }
}

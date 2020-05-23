namespace Nacos.Config.Http
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class ServerHttpAgent : HttpAgent
    {
        private readonly ILogger _logger;
        private readonly NacosOptions _options;
        private readonly IHttpClientFactory _clientFactory;

        public ServerHttpAgent(
            ILoggerFactory loggerFactory, 
            IOptions<NacosOptions> optionsAccs, 
            IHttpClientFactory clientFactory)
        {
            _logger = loggerFactory.CreateLogger<ServerHttpAgent>();
            _options = optionsAccs.Value;
            _clientFactory = clientFactory;
        }

        public override async Task<HttpResponseMessage> ReqApiAsync(HttpMethod httpMethod, string path, Dictionary<string, string> headers, Dictionary<string, string> paramValues, int timeout)
        {
            var client = _clientFactory.CreateClient(ConstValue.ClientName);
            client.Timeout = TimeSpan.FromMilliseconds(timeout);

            var requestMessage = new HttpRequestMessage();
            requestMessage.Method = httpMethod;

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

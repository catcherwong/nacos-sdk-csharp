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
        private readonly ServerListManager _serverListMgr;

        public ServerHttpAgent(
            ILoggerFactory loggerFactory, 
            IOptions<NacosOptions> optionsAccs, 
            IHttpClientFactory clientFactory)
        {
            _logger = loggerFactory.CreateLogger<ServerHttpAgent>();
            _options = optionsAccs.Value;
            _clientFactory = clientFactory;

            _serverListMgr = new ServerListManager(_options);
        }

        public override string AbstGetName() => _serverListMgr.GetName();

        public override string AbstGetNamespace() => _serverListMgr.GetNamespace();

        public override string AbstGetTenant() => _serverListMgr.GetTenant();

        public override async Task<HttpResponseMessage> ReqApiAsync(HttpMethod httpMethod, string path, Dictionary<string, string> headers, Dictionary<string, string> paramValues, int timeout)
        {
            var client = _clientFactory.CreateClient(ConstValue.ClientName);
            client.Timeout = TimeSpan.FromMilliseconds(timeout);

            var requestMessage = new HttpRequestMessage();
            requestMessage.Method = httpMethod;

            var currentServerAddr = _serverListMgr.GetCurrentServerAddr();

            var requestUrl = GetUrl(currentServerAddr, path);

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
                    requestMessage.RequestUri = new Uri($"{requestUrl}?{query}");
                }
            }

            HttpAgentCommon.BuildHeader(requestMessage, headers);
            HttpAgentCommon.BuildSpasHeaders(requestMessage, paramValues, _options.AccessKey, _options.SecretKey);

            var responseMessage = await client.SendAsync(requestMessage);

            if (responseMessage.StatusCode == System.Net.HttpStatusCode.InternalServerError
                || responseMessage.StatusCode == System.Net.HttpStatusCode.BadGateway
                || responseMessage.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            {
                _logger.LogError("[NACOS ConnectException] currentServerAddr: {0}, httpCode: {1}", _serverListMgr.GetCurrentServerAddr(), responseMessage.StatusCode);
            }
            else
            {
                _serverListMgr.UpdateCurrentServerAddr(currentServerAddr);
                return responseMessage;
            }

            throw new System.Net.Http.HttpRequestException($"no available server, currentServerAddr : {currentServerAddr}");
        }

        private string GetUrl(string serverAddr, string relativePath)
        {           
            return  $"{serverAddr}{relativePath}";
        }
    }
}

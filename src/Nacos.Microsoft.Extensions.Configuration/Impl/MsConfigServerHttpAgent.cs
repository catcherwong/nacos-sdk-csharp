namespace Nacos.Microsoft.Extensions.Configuration
{
    using Nacos.Config;
    using Nacos.Config.Http;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class MsConfigServerHttpAgent : HttpAgent
    {
        private readonly NacosOptions _options;
        private readonly ServerListManager _serverListMgr;

        public MsConfigServerHttpAgent(NacosOptions options)
        {
            _options = options;
            _serverListMgr = new ServerListManager(_options);
        }

        public override string AbstGetName() => _serverListMgr.GetName();

        public override string AbstGetNamespace() => _serverListMgr.GetNamespace();

        public override string AbstGetTenant() => _serverListMgr.GetTenant();

        public override async Task<HttpResponseMessage> ReqApiAsync(HttpMethod httpMethod, string path, Dictionary<string, string> headers, Dictionary<string, string> paramValues, int timeout)
        {
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(timeout);

                var requestMessage = new HttpRequestMessage
                {
                    Method = httpMethod
                };

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
                    System.Diagnostics.Trace.TraceError("[NACOS ConnectException] currentServerAddr: {0}, httpCode: {1}", _serverListMgr.GetCurrentServerAddr(), responseMessage.StatusCode);
                }
                else
                {
                    _serverListMgr.UpdateCurrentServerAddr(currentServerAddr);
                    return responseMessage;
                }

                throw new System.Net.Http.HttpRequestException($"no available server, currentServerAddr : {currentServerAddr}");
            }
        }

        private string GetUrl(string serverAddr, string relativePath)
        {
            return $"{serverAddr}{relativePath}";
        }
    }
}

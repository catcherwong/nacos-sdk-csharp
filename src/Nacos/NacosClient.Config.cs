namespace Nacos
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Nacos.Exceptions;
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public partial class NacosClient : INacosClient
    {
        private readonly ILogger _logger;
        private readonly NacosOptions _options;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILocalConfigInfoProcessor _processor;

        public NacosClient(
            ILoggerFactory loggerFactory
            , IOptionsMonitor<NacosOptions> optionAccs
            , IHttpClientFactory clientFactory
            , ILocalConfigInfoProcessor processor)
        {
            this._logger = loggerFactory.CreateLogger<NacosClient>();
            this._options = optionAccs.CurrentValue;
            this._clientFactory = clientFactory;
            this._processor = processor;
        }

        public async Task<string> GetConfigAsync(GetConfigRequest request)
        {
            if (!request.IsValid())
            {
                throw new RequestInValidException("request 参数不合法");
            }

            request.Tenant = string.IsNullOrWhiteSpace(request.Tenant) ? _options.Namespace : request.Tenant;
            request.Group = string.IsNullOrWhiteSpace(request.Group) ? ConstValue.DefaultGroup : request.Group;

            // 先从本地缓存取
            var config = await _processor.GetFailoverAsync(request.DataId, request.Group, request.Tenant);

            if (!string.IsNullOrWhiteSpace(config))
            {
                _logger.LogInformation($"[get-config] get failover ok, dataId={request.DataId}, group={request.Group}, tenant={request.Tenant}, config ={config}");
                return config;
            }

            try
            {
                config = await DoGetConfigAsync(request);
            }
            catch (NacosException ex)
            {
                // 没有权限的异常，直接抛出，不去snapshot找
                if (ConstValue.NO_RIGHT == ex.ErrorCode)
                {
                    throw;
                }

                _logger.LogWarning($"[get-config] get from server error, dataId={request.DataId}, group={request.Group}, tenant={request.Tenant}, msg={ex.Message}");
            }

            if (!string.IsNullOrWhiteSpace(config))
            {
                _logger.LogInformation($"[get-config] content from server {config}, dataId={request.DataId}, group={request.Group}, tenant={request.Tenant}");
                await _processor.SaveSnapshotAsync(request.DataId, request.Group, request.Tenant, config);
                return config;
            }

            config = await _processor.GetSnapshotAync(request.DataId, request.Group, request.Tenant);

            return config;
        }

        private async Task<string> DoGetConfigAsync(GetConfigRequest request)
        {
            var responseMessage = await DoRequestAsync(HttpMethod.Get, $"{_options.EndPoint}/nacos/v1/cs/configs", request.ToQueryString());

            switch (responseMessage.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    var result = await responseMessage.Content.ReadAsStringAsync();
                    return result;
                case System.Net.HttpStatusCode.NotFound:
                    await _processor.SaveSnapshotAsync(request.DataId, request.Group, request.Tenant, null);
                    return null;
                case System.Net.HttpStatusCode.Forbidden:
                    throw new NacosException(ConstValue.NO_RIGHT, $"Insufficient privilege.");
                default:
                    throw new NacosException((int)responseMessage.StatusCode, responseMessage.StatusCode.ToString());
            }
        }

        public async Task<bool> PublishConfigAsync(PublishConfigRequest request)
        {
            if (!request.IsValid())
            {
                throw new RequestInValidException("request 参数不合法");
            }

            request.Tenant = string.IsNullOrWhiteSpace(request.Tenant) ? _options.Namespace : request.Tenant;
            request.Group = string.IsNullOrWhiteSpace(request.Group) ? ConstValue.DefaultGroup : request.Group;

            var responseMessage = await DoRequestAsync(HttpMethod.Post, $"{_options.EndPoint}/nacos/v1/cs/configs", request.ToQueryString());

            switch (responseMessage.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    _logger.LogInformation($"[publish-single] ok, dataId={request.DataId}, group={request.Group}, tenant={request.Tenant}, config={request.Content}");
                    var result = await responseMessage.Content.ReadAsStringAsync();
                    return result.Equals("true", StringComparison.OrdinalIgnoreCase);
                case System.Net.HttpStatusCode.Forbidden:
                    _logger.LogWarning($"[publish-single] error, dataId={request.DataId}, group={request.Group}, tenant={request.Tenant}, code={(int)responseMessage.StatusCode} msg={responseMessage.StatusCode.ToString()}");
                    throw new NacosException(ConstValue.NO_RIGHT, $"Insufficient privilege.");
                default:
                    _logger.LogWarning($"[publish-single] error, dataId={request.DataId}, group={request.Group}, tenant={request.Tenant}, code={(int)responseMessage.StatusCode} msg={responseMessage.StatusCode.ToString()}");
                    throw new NacosException((int)responseMessage.StatusCode, responseMessage.StatusCode.ToString());
            }
        }

        public async Task<bool> RemoveConfigAsync(RemoveConfigRequest request)
        {
            if (!request.IsValid())
            {
                throw new RequestInValidException("request 参数不合法");
            }

            request.Tenant = string.IsNullOrWhiteSpace(request.Tenant) ? _options.Namespace : request.Tenant;
            request.Group = string.IsNullOrWhiteSpace(request.Group) ? ConstValue.DefaultGroup : request.Group;

            var responseMessage = await DoRequestAsync(HttpMethod.Delete, $"{_options.EndPoint}/nacos/v1/cs/configs", request.ToQueryString());

            switch (responseMessage.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    _logger.LogInformation($"[remove] ok, dataId={request.DataId}, group={request.Group}, tenant={request.Tenant}");
                    var result = await responseMessage.Content.ReadAsStringAsync();
                    return result.Equals("true", StringComparison.OrdinalIgnoreCase);
                case System.Net.HttpStatusCode.Forbidden:
                    _logger.LogWarning($"[remove] error, dataId={request.DataId}, group={request.Group}, tenant={request.Tenant}, code={(int)responseMessage.StatusCode} msg={responseMessage.StatusCode.ToString()}");
                    throw new NacosException(ConstValue.NO_RIGHT, $"Insufficient privilege.");
                default:
                    _logger.LogWarning($"[remove] error, dataId={request.DataId}, group={request.Group}, tenant={request.Tenant}, code={(int)responseMessage.StatusCode} msg={responseMessage.StatusCode.ToString()}");
                    throw new NacosException((int)responseMessage.StatusCode, responseMessage.StatusCode.ToString());
            }
        }

        public async Task ListenerConfigAsync(ListenerConfigRequest request)
        {
            if (!request.IsValid())
            {
                throw new RequestInValidException("request 参数不合法");
            }

            request.Tenant = string.IsNullOrWhiteSpace(request.Tenant) ? _options.Namespace : request.Tenant;
            request.Group = string.IsNullOrWhiteSpace(request.Group) ? ConstValue.DefaultGroup : request.Group;

            var config = string.Empty;

            do
            {
                request.Content = config;

                try
                {
                    var client = _clientFactory.CreateClient(ConstValue.ClientName);
                    // 比长轮训等待时间多一点
                    client.Timeout = TimeSpan.FromSeconds(ConstValue.LongPullingTimeout + 10);

                    var stringContent = new StringContent(request.ToQueryString());
                    stringContent.Headers.TryAddWithoutValidation("Long-Pulling-Timeout", (ConstValue.LongPullingTimeout * 1000).ToString());
                    stringContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

                    var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{_options.EndPoint}/nacos/v1/cs/configs/listener")
                    {
                        Content = stringContent
                    };

                    var responseMessage = await client.SendAsync(requestMessage);

                    switch (responseMessage.StatusCode)
                    {
                        case System.Net.HttpStatusCode.OK:
                            var content = await responseMessage.Content.ReadAsStringAsync();
                            config = await HandleContentAsync(content, request);
                            break;
                        case System.Net.HttpStatusCode.Forbidden:
                            _logger.LogWarning($"[listener] error, dataId={request.DataId}, group={request.Group}, tenant={request.Tenant}, code={(int)responseMessage.StatusCode} msg={responseMessage.StatusCode.ToString()}");
                            throw new NacosException(ConstValue.NO_RIGHT, $"Insufficient privilege.");
                        default:
                            _logger.LogWarning($"[listener] error, dataId={request.DataId}, group={request.Group}, tenant={request.Tenant}, code={(int)responseMessage.StatusCode} msg={responseMessage.StatusCode.ToString()}");
                            throw new NacosException((int)responseMessage.StatusCode, responseMessage.StatusCode.ToString());
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"[listener] error, dataId={request.DataId}, group={request.Group}, tenant={request.Tenant}");
                    Thread.Sleep(1000);
                }
            } while (true);
        }

        private async Task<string> HandleContentAsync(string content, ListenerConfigRequest request)
        {
            string config;

            if (!string.IsNullOrWhiteSpace(content))
            {
                config = await DoGetConfigAsync(new GetConfigRequest
                {
                    DataId = request.DataId,
                    Group = request.Group,
                    Tenant = request.Tenant
                });

                await _processor.SaveSnapshotAsync(request.DataId, request.Group, request.Tenant, config);
            }
            else
            {
                config = await _processor.GetFailoverAsync(request.DataId, request.Group, request.Tenant);
            }

            return config;
        }
    }
}

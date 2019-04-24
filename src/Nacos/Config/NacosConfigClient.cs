namespace Nacos
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Nacos.Exceptions;
    using Nacos.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public class NacosConfigClient : INacosConfigClient
    {
        private readonly ILogger _logger;
        private readonly NacosOptions _options;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILocalConfigInfoProcessor _processor;
        private readonly List<Listener> listeners;
        private readonly ServerAddressManager _serverAddressManager;

        public NacosConfigClient(
            ILoggerFactory loggerFactory
            , IOptionsMonitor<NacosOptions> optionAccs
            , IHttpClientFactory clientFactory
            , ILocalConfigInfoProcessor processor)
        {
            this._logger = loggerFactory.CreateLogger<NacosConfigClient>();
            this._options = optionAccs.CurrentValue;
            this._clientFactory = clientFactory;
            this._processor = processor;

            this.listeners = new List<Listener>();
            this._serverAddressManager = new ServerAddressManager(_options);
        }

        public async Task<string> GetConfigAsync(GetConfigRequest request)
        {
            if (request == null) throw new NacosException(ConstValue.CLIENT_INVALID_PARAM, "request param invalid");

            request.Tenant = string.IsNullOrWhiteSpace(request.Tenant) ? _options.Namespace : request.Tenant;
            request.Group = string.IsNullOrWhiteSpace(request.Group) ? ConstValue.DefaultGroup : request.Group;

            request.CheckParam();

            // read from local cache at first
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
            var responseMessage = await _clientFactory.DoRequestAsync(HttpMethod.Get, $"{GetBaseUrl()}/nacos/v1/cs/configs", request.ToQueryString(), _options.DefaultTimeOut);

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
            if (request == null) throw new NacosException(ConstValue.CLIENT_INVALID_PARAM, "request param invalid");

            request.Tenant = string.IsNullOrWhiteSpace(request.Tenant) ? _options.Namespace : request.Tenant;
            request.Group = string.IsNullOrWhiteSpace(request.Group) ? ConstValue.DefaultGroup : request.Group;

            request.CheckParam();

            var responseMessage = await _clientFactory.DoRequestAsync(HttpMethod.Post, $"{GetBaseUrl()}/nacos/v1/cs/configs", request.ToQueryString(), _options.DefaultTimeOut);

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
                    return false;
            }
        }

        public async Task<bool> RemoveConfigAsync(RemoveConfigRequest request)
        {
            if (request == null) throw new NacosException(ConstValue.CLIENT_INVALID_PARAM, "request param invalid");

            request.Tenant = string.IsNullOrWhiteSpace(request.Tenant) ? _options.Namespace : request.Tenant;
            request.Group = string.IsNullOrWhiteSpace(request.Group) ? ConstValue.DefaultGroup : request.Group;

            request.CheckParam();

            var responseMessage = await _clientFactory.DoRequestAsync(HttpMethod.Delete, $"{GetBaseUrl()}/nacos/v1/cs/configs", request.ToQueryString(), _options.DefaultTimeOut);

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
                    return false;
            }
        }

        public Task AddListenerAsync(AddListenerRequest request)
        {
            if (request == null) throw new NacosException(ConstValue.CLIENT_INVALID_PARAM, "request param invalid");

            if (string.IsNullOrWhiteSpace(request.Tenant)) request.Tenant = _options.Namespace;
            if (string.IsNullOrWhiteSpace(request.Group)) request.Group = ConstValue.DefaultGroup;

            request.CheckParam();

            var name = BuildName(request.Tenant, request.Group, request.DataId);

            if (listeners.Any(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogWarning($"[add-listener] error, {name} has been added.");
                return Task.CompletedTask;
            }

            Timer timer = new Timer(async x =>
            {
                await PollingAsync(x);
#if !DEBUG
            }, request, 0, _options.ListenInterval);
#else
            }, request, 0, 8000);
#endif

            listeners.Add(new Listener(name, timer));

            return Task.CompletedTask;
        }

        public Task RemoveListenerAsync(RemoveListenerRequest request)
        {
            if (request == null) throw new NacosException(ConstValue.CLIENT_INVALID_PARAM, "request param invalid");

            if (string.IsNullOrWhiteSpace(request.Tenant)) request.Tenant = _options.Namespace;
            if (string.IsNullOrWhiteSpace(request.Group)) request.Group = ConstValue.DefaultGroup;

            request.CheckParam();

            var name = BuildName(request.Tenant, request.Group, request.DataId);

            if (!listeners.Any(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogWarning($"[remove-listener] error, {name} was not added.");
                return Task.CompletedTask;
            }

            var list = listeners.Where(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).ToList();

            // clean timer
            foreach (var item in list)
            {
                item.Timer.Dispose();
                item.Timer = null;
            }

            // remove listeners
            listeners.RemoveAll(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            foreach (var cb in request.Callbacks)
            {
                try
                {
                    cb();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"[remove-listener] call back throw exception, dataId={request.DataId}, group={request.Group}, tenant={request.Tenant}");
                }
            }

            return Task.CompletedTask;
        }

        private string BuildName(string tenant, string group, string dataId)
        {
            return $"{tenant}-{group}-{dataId}";
        }

        private async Task PollingAsync(object requestInfo)
        {
            var request = (AddListenerRequest)requestInfo;

            // read the last config
            var lastConfig = await _processor.GetSnapshotAync(request.DataId, request.Group, request.Tenant);
            request.Content = lastConfig;

            try
            {
                var client = _clientFactory.CreateClient(ConstValue.ClientName);

                // longer than long pulling timeout
                client.Timeout = TimeSpan.FromSeconds(ConstValue.LongPullingTimeout + 10);

                var stringContent = new StringContent(request.ToQueryString());
                stringContent.Headers.TryAddWithoutValidation("Long-Pulling-Timeout", (ConstValue.LongPullingTimeout * 1000).ToString());
                stringContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{GetBaseUrl()}/nacos/v1/cs/configs/listener")
                {
                    Content = stringContent
                };

                var responseMessage = await client.SendAsync(requestMessage);

                switch (responseMessage.StatusCode)
                {
                    case System.Net.HttpStatusCode.OK:
                        var content = await responseMessage.Content.ReadAsStringAsync();
                        await ConfigChangeAsync(content, request);
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
            }
        }

        private async Task ConfigChangeAsync(string content, AddListenerRequest request)
        {
            // config was changed
            if (!string.IsNullOrWhiteSpace(content))
            {
                var config = await DoGetConfigAsync(new GetConfigRequest
                {
                    DataId = request.DataId,
                    Group = request.Group,
                    Tenant = request.Tenant
                });

                // update local cache
                await _processor.SaveSnapshotAsync(request.DataId, request.Group, request.Tenant, config);

                // callback
                foreach (var cb in request.Callbacks)
                {
                    try
                    {
                        cb(config);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"[listener] call back throw exception, dataId={request.DataId}, group={request.Group}, tenant={request.Tenant}");
                    }
                }
            }
        }

        private string GetBaseUrl()
        {
            var hostAndPort = _serverAddressManager.GetCurrentServer();
            return $"http://{hostAndPort}";
        }
    }
}

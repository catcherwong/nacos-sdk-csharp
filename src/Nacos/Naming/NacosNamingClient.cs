﻿namespace Nacos
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Nacos.Exceptions;
    using Nacos.Utilities;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class NacosNamingClient : INacosNamingClient
    {
        private readonly ILogger _logger;
        private readonly NacosOptions _options;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ServerAddressManager _serverAddressManager;

        public NacosNamingClient(
            ILoggerFactory loggerFactory
            , IOptionsMonitor<NacosOptions> optionAccs
            , IHttpClientFactory clientFactory)
        {
            this._logger = loggerFactory.CreateLogger<NacosNamingClient>();
            this._options = optionAccs.CurrentValue;
            this._clientFactory = clientFactory;

            this._serverAddressManager = new ServerAddressManager(_options);
        }

        #region Instance
        public async Task<bool> RegisterInstanceAsync(RegisterInstanceRequest request)
        {
            if (request == null) throw new NacosException(ConstValue.CLIENT_INVALID_PARAM, "request param invalid");

            request.CheckParam();

            var responseMessage = await _clientFactory.DoRequestAsync(HttpMethod.Post, $"{GetBaseUrl()}{RequestPathValue.INSTANCE}", request.ToQueryString(), _options.DefaultTimeOut);

            switch (responseMessage.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    var result = await responseMessage.Content.ReadAsStringAsync();
                    if (result.Equals("ok", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning($"[client.RegisterInstance] server return {result} ");
                        return false;
                    }
                default:
                    _logger.LogWarning($"[client.RegisterInstance] Register an instance to service failed {responseMessage.StatusCode.ToString()}");
                    throw new NacosException((int)responseMessage.StatusCode, $"Register an instance to service failed {responseMessage.StatusCode.ToString()}");
            }
        }

        public async Task<bool> RemoveInstanceAsync(RemoveInstanceRequest request)
        {
            if (request == null) throw new NacosException(ConstValue.CLIENT_INVALID_PARAM, "request param invalid");

            request.CheckParam();

            var responseMessage = await _clientFactory.DoRequestAsync(HttpMethod.Delete, $"{GetBaseUrl()}{RequestPathValue.INSTANCE}", request.ToQueryString(), _options.DefaultTimeOut);

            switch (responseMessage.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    var result = await responseMessage.Content.ReadAsStringAsync();
                    if (result.Equals("ok", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning($"[client.RemoveInstance] server return {result} ");
                        return false;
                    }
                default:
                    _logger.LogWarning($"[client.RemoveInstance] Delete instance from service failed {responseMessage.StatusCode.ToString()}");
                    throw new NacosException((int)responseMessage.StatusCode, $"Delete instance from service failed {responseMessage.StatusCode.ToString()}");
            }
        }

        public async Task<bool> ModifyInstanceAsync(ModifyInstanceRequest request)
        {
            if (request == null) throw new NacosException(ConstValue.CLIENT_INVALID_PARAM, "request param invalid");

            request.CheckParam();

            var responseMessage = await _clientFactory.DoRequestAsync(HttpMethod.Put, $"{GetBaseUrl()}{RequestPathValue.INSTANCE}", request.ToQueryString(), _options.DefaultTimeOut);

            switch (responseMessage.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    var result = await responseMessage.Content.ReadAsStringAsync();
                    if (result.Equals("ok", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning($"[client.ModifyInstance] server return {result} ");
                        return false;
                    }
                default:
                    _logger.LogWarning($"[client.ModifyInstance] Modify an instance of service failed {responseMessage.StatusCode.ToString()}");
                    throw new NacosException((int)responseMessage.StatusCode, $"Modify an instance of service failed {responseMessage.StatusCode.ToString()}");
            }
        }

        public async Task<ListInstancesResult> ListInstancesAsync(ListInstancesRequest request)
        {
            if (request == null) throw new NacosException(ConstValue.CLIENT_INVALID_PARAM, "request param invalid");

            request.CheckParam();

            var responseMessage = await _clientFactory.DoRequestAsync(HttpMethod.Get, $"{GetBaseUrl()}{RequestPathValue.INSTANCE_LIST}", request.ToQueryString(), _options.DefaultTimeOut);

            switch (responseMessage.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    var result = await responseMessage.Content.ReadAsStringAsync();
                    var obj = result.ToObj<ListInstancesResult>();
                    return obj;
                default:
                    _logger.LogWarning($"[client.ListInstances] Query instance list of service failed {responseMessage.StatusCode.ToString()}");
                    throw new NacosException((int)responseMessage.StatusCode, $"Query instance list of service failed {responseMessage.StatusCode.ToString()}");
            }
        }

        public async Task<GetInstanceResult> GetInstanceAsync(GetInstanceRequest request)
        {
            if (request == null) throw new NacosException(ConstValue.CLIENT_INVALID_PARAM, "request param invalid");

            request.CheckParam();

            var responseMessage = await _clientFactory.DoRequestAsync(HttpMethod.Get, $"{GetBaseUrl()}{RequestPathValue.INSTANCE}", request.ToQueryString(), _options.DefaultTimeOut);

            switch (responseMessage.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    var result = await responseMessage.Content.ReadAsStringAsync();
                    var obj = result.ToObj<GetInstanceResult>();
                    return obj;
                default:
                    _logger.LogWarning($"[client.GetInstance] Query instance details of service failed {responseMessage.StatusCode.ToString()}");
                    throw new NacosException((int)responseMessage.StatusCode, $"Query instance details of service failed {responseMessage.StatusCode.ToString()}");
            }
        }

        public async Task<bool> SendHeartbeatAsync(SendHeartbeatRequest request)
        {
            if (request == null) throw new NacosException(ConstValue.CLIENT_INVALID_PARAM, "request param invalid");

            request.CheckParam();

            var responseMessage = await _clientFactory.DoRequestAsync(HttpMethod.Put, $"{GetBaseUrl()}{RequestPathValue.INSTANCE_BEAT}", request.ToQueryString(), _options.DefaultTimeOut);

            switch (responseMessage.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    var result = await responseMessage.Content.ReadAsStringAsync();
                    var jObj = Newtonsoft.Json.Linq.JObject.Parse(result);

                    if (jObj.ContainsKey("code"))
                    {
                        int code = int.Parse(jObj["code"].ToString());

                        var flag = code == 10200;

                        if (!flag) _logger.LogWarning($"[client.SendHeartbeat] server return {result} ");

                        return flag;
                    }
                    else
                    {
                        _logger.LogWarning($"[client.SendHeartbeat] server return {result} ");
                        return false;
                    }

                default:
                    _logger.LogWarning($"[client.SendHeartbeat] Send instance beat failed {responseMessage.StatusCode.ToString()}");
                    throw new NacosException((int)responseMessage.StatusCode, $"Send instance beat failed {responseMessage.StatusCode.ToString()}");
            }
        }

        public async Task<bool> ModifyInstanceHealthStatusAsync(ModifyInstanceHealthStatusRequest request)
        {
            if (request == null) throw new NacosException(ConstValue.CLIENT_INVALID_PARAM, "request param invalid");

            request.CheckParam();

            var responseMessage = await _clientFactory.DoRequestAsync(HttpMethod.Put, $"{GetBaseUrl()}{RequestPathValue.INSTANCE_HEALTH}", request.ToQueryString(), _options.DefaultTimeOut);

            switch (responseMessage.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    var result = await responseMessage.Content.ReadAsStringAsync();
                    if (result.Equals("ok", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning($"[client.ModifyInstanceHealthStatus] server return {result} ");
                        return false;
                    }
                case System.Net.HttpStatusCode.BadRequest:
                    _logger.LogWarning($"[client.ModifyInstanceHealthStatus] health check is still working {responseMessage.StatusCode.ToString()}");
                    return false;
                default:
                    _logger.LogWarning($"[client.ModifyInstanceHealthStatus] Update instance health status failed {responseMessage.StatusCode.ToString()}");
                    throw new NacosException((int)responseMessage.StatusCode, $"Update instance health status failed {responseMessage.StatusCode.ToString()}");
            }
        }
        #endregion

        #region Metrics
        public async Task<GetMetricsResult> GetMetricsAsync()
        {
            var responseMessage = await _clientFactory.DoRequestAsync(HttpMethod.Get, $"{GetBaseUrl()}{RequestPathValue.METRICS}", timeOut: _options.DefaultTimeOut);

            switch (responseMessage.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    var result = await responseMessage.Content.ReadAsStringAsync();
                    var obj = result.ToObj<GetMetricsResult>();
                    return obj;
                default:
                    _logger.LogWarning($"[client.GetMetrics] Query system metrics failed {responseMessage.StatusCode.ToString()}");
                    throw new NacosException((int)responseMessage.StatusCode, $"Query system metrics failed {responseMessage.StatusCode.ToString()}");
            }
        }
        #endregion

        #region Services
        public async Task<bool> CreateServiceAsync(CreateServiceRequest request)
        {
            if (request == null) throw new NacosException(ConstValue.CLIENT_INVALID_PARAM, "request param invalid");

            request.CheckParam();

            var responseMessage = await _clientFactory.DoRequestAsync(HttpMethod.Post, $"{GetBaseUrl()}{RequestPathValue.SERVICE}", request.ToQueryString(), _options.DefaultTimeOut);

            switch (responseMessage.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    var result = await responseMessage.Content.ReadAsStringAsync();
                    if (result.Equals("ok", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning($"[client.CreateService] server return {result} ");
                        return false;
                    }
                default:
                    _logger.LogWarning($"[client.CreateService] Create service failed {responseMessage.StatusCode.ToString()}");
                    throw new NacosException((int)responseMessage.StatusCode, $"Create service failed {responseMessage.StatusCode.ToString()}");
            }
        }

        public async Task<bool> RemoveServiceAsync(RemoveServiceRequest request)
        {
            if (request == null) throw new NacosException(ConstValue.CLIENT_INVALID_PARAM, "request param invalid");

            request.CheckParam();

            var responseMessage = await _clientFactory.DoRequestAsync(HttpMethod.Delete, $"{GetBaseUrl()}{RequestPathValue.SERVICE}", request.ToQueryString(), _options.DefaultTimeOut);

            switch (responseMessage.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    var result = await responseMessage.Content.ReadAsStringAsync();
                    if (result.Equals("ok", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning($"[client.RemoveService] server return {result} ");
                        return false;
                    }
                default:
                    _logger.LogWarning($"[client.RemoveService] Delete a service failed {responseMessage.StatusCode.ToString()}");
                    throw new NacosException((int)responseMessage.StatusCode, $"Delete a service failed {responseMessage.StatusCode.ToString()}");
            }
        }

        public async Task<bool> ModifyServiceAsync(ModifyServiceRequest request)
        {
            if (request == null) throw new NacosException(ConstValue.CLIENT_INVALID_PARAM, "request param invalid");

            request.CheckParam();

            var responseMessage = await _clientFactory.DoRequestAsync(HttpMethod.Put, $"{GetBaseUrl()}{RequestPathValue.SERVICE}", request.ToQueryString(), _options.DefaultTimeOut);

            switch (responseMessage.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    var result = await responseMessage.Content.ReadAsStringAsync();
                    if (result.Equals("ok", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning($"[client.ModifyService] server return {result} ");
                        return false;
                    }
                default:
                    _logger.LogWarning($"[client.ModifyService] Update a service failed {responseMessage.StatusCode.ToString()}");
                    throw new NacosException((int)responseMessage.StatusCode, $"Update a service failed {responseMessage.StatusCode.ToString()}");
            }
        }

        public async Task<GetServiceResult> GetServiceAsync(GetServiceRequest request)
        {
            if (request == null) throw new NacosException(ConstValue.CLIENT_INVALID_PARAM, "request param invalid");

            request.CheckParam();

            var responseMessage = await _clientFactory.DoRequestAsync(HttpMethod.Get, $"{GetBaseUrl()}{RequestPathValue.SERVICE}", request.ToQueryString(), _options.DefaultTimeOut);

            switch (responseMessage.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    var result = await responseMessage.Content.ReadAsStringAsync();
                    var obj = result.ToObj<GetServiceResult>();
                    return obj;
                default:
                    _logger.LogWarning($"[client.GetService] Query a service failed {responseMessage.StatusCode.ToString()}");
                    throw new NacosException((int)responseMessage.StatusCode, $"Query a service failed {responseMessage.StatusCode.ToString()}");
            }
        }

        public async Task<ListServicesResult> ListServicesAsync(ListServicesRequest request)
        {
            if (request == null) throw new NacosException(ConstValue.CLIENT_INVALID_PARAM, "request param invalid");

            request.CheckParam();

            var responseMessage = await _clientFactory.DoRequestAsync(HttpMethod.Get, $"{GetBaseUrl()}{RequestPathValue.SERVICE_LIST}", request.ToQueryString(), _options.DefaultTimeOut);

            switch (responseMessage.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    var result = await responseMessage.Content.ReadAsStringAsync();
                    var obj = result.ToObj<ListServicesResult>();
                    return obj;
                default:
                    _logger.LogWarning($"[client.ListServices] Query service list failed {responseMessage.StatusCode.ToString()}");
                    throw new NacosException((int)responseMessage.StatusCode, $"Query service list failed {responseMessage.StatusCode.ToString()}");
            }
        }
        #endregion

        #region Switches
        public async Task<GetSwitchesResult> GetSwitchesAsync()
        {
            var responseMessage = await _clientFactory.DoRequestAsync(HttpMethod.Get, $"{GetBaseUrl()}{RequestPathValue.SWITCHES}", timeOut: _options.DefaultTimeOut);

            switch (responseMessage.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    var result = await responseMessage.Content.ReadAsStringAsync();
                    var obj = result.ToObj<GetSwitchesResult>();
                    return obj;
                default:
                    _logger.LogWarning($"[client.GetSwitches] Query system switches failed {responseMessage.StatusCode.ToString()}");
                    throw new NacosException((int)responseMessage.StatusCode, $"Query system switches failed {responseMessage.StatusCode.ToString()}");
            }
        }

        public async Task<bool> ModifySwitchesAsync(ModifySwitchesRequest request)
        {
            if (request == null) throw new NacosException(ConstValue.CLIENT_INVALID_PARAM, "request param invalid");

            request.CheckParam();

            var responseMessage = await _clientFactory.DoRequestAsync(HttpMethod.Put, $"{GetBaseUrl()}{RequestPathValue.SWITCHES}", request.ToQueryString());

            switch (responseMessage.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    var result = await responseMessage.Content.ReadAsStringAsync();
                    if (result.Equals("ok", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning($"[client.ModifySwitches] server return {result} ");
                        return false;
                    }
                default:
                    _logger.LogWarning($"[client.ModifySwitches] Update system switch failed {responseMessage.StatusCode.ToString()}");
                    throw new NacosException((int)responseMessage.StatusCode, $"Update system switch failed {responseMessage.StatusCode.ToString()}");
            }
        }

        #endregion

        #region Cluster
        public async Task<ListClusterServersResult> ListClusterServersAsync(ListClusterServersRequest request)
        {
            if (request == null) throw new NacosException(ConstValue.CLIENT_INVALID_PARAM, "request param invalid");

            request.CheckParam();

            var responseMessage = await _clientFactory.DoRequestAsync(HttpMethod.Get, $"{GetBaseUrl()}{RequestPathValue.SERVERS}", request.ToQueryString(), _options.DefaultTimeOut);

            switch (responseMessage.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    var result = await responseMessage.Content.ReadAsStringAsync();
                    var obj = result.ToObj<ListClusterServersResult>();
                    return obj;
                default:
                    _logger.LogWarning($"[client.ListClusterServers] Query server list failed {responseMessage.StatusCode.ToString()}");
                    throw new NacosException((int)responseMessage.StatusCode, $"Query server list failed {responseMessage.StatusCode.ToString()}");
            }
        }

        public async Task<GetCurrentClusterLeaderResult> GetCurrentClusterLeaderAsync()
        {
            var responseMessage = await _clientFactory.DoRequestAsync(HttpMethod.Get, $"{GetBaseUrl()}{RequestPathValue.LEADER}");

            switch (responseMessage.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    var result = await responseMessage.Content.ReadAsStringAsync();
                    var leader = result.GetPropValue("leader");
                    var obj = leader.ToObj<GetCurrentClusterLeaderResult>();
                    return obj;
                default:
                    _logger.LogWarning($"[client.GetCurrentClusterLeader] query the leader of current cluster failed {responseMessage.StatusCode.ToString()}");
                    throw new NacosException((int)responseMessage.StatusCode, $"query the leader of current cluster failed {responseMessage.StatusCode.ToString()}");
            }
        }
        #endregion

        private string GetBaseUrl()
        {
            var hostAndPort = _serverAddressManager.GetCurrentServer();
            return hostAndPort;
        }
    }
}

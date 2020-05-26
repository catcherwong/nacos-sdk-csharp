﻿namespace Nacos.Naming.Http
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Nacos.Config.Http;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public class NamingProxy : IDisposable
    {
        private readonly ILogger _logger;
        private readonly NacosOptions _options;
        private readonly IHttpClientFactory _clientFactory;
        private List<string> _serverUrls;

        private string _namespace;
        private string _endpoint;
        private string _nacosDomain;
        private int _serverPort = 8848;

        private long _lastSrvRefTime = 0;
        private readonly int _vipSrvRefInterMillis = 30 * 1000;

        private Timer t;

        public NamingProxy(
          ILoggerFactory loggerFactory,
          IOptions<NacosOptions> optionsAccs,
          IHttpClientFactory clientFactory)
        {
            _logger = loggerFactory.CreateLogger<NamingProxy>();
            _options = optionsAccs.Value;
            _clientFactory = clientFactory;

            _serverUrls = new List<string>();
            _namespace = _options.Namespace;
            _endpoint = _options.EndPoint;

            var serverAddresses = _options.ServerAddresses;
            
            if (serverAddresses != null && serverAddresses.Any())
            {
                foreach (var item in serverAddresses)
                {
                    // here only trust the input server addresses of user
                    _serverUrls.Add(item.TrimEnd('/'));
                }

                if (_serverUrls.Count == 1) _nacosDomain = _serverUrls.First();
            }

            InitRefreshTask();
        }

        private void InitRefreshTask()
        {
            t = new Timer(async x =>
            {
                await RefreshSrvIfNeedAsync();
            }, null, 0, _vipSrvRefInterMillis);

            RefreshSrvIfNeedAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private async Task RefreshSrvIfNeedAsync()
        {
            try
            {
                if (_serverUrls != null && _serverUrls.Count > 0)
                {
                    _logger.LogDebug("server list provided by user: {0}", string.Join(",", _serverUrls));
                    return;
                }

                var list = await GetServerListFromEndpointAsync();

                if (list == null || list.Count <= 0)
                {
                    throw new Exception("Can not acquire Nacos list");
                }

                List<string> newServerAddrList = new List<string>();

                foreach (var server in list)
                {
                    if (server.StartsWith(ConstValue.HTTPS, StringComparison.OrdinalIgnoreCase)
                        || server.StartsWith(ConstValue.HTTP, StringComparison.OrdinalIgnoreCase))
                    {
                        newServerAddrList.Add(server);
                    }
                    else
                    {
                        newServerAddrList.Add($"{ConstValue.HTTP}{server}");
                    }
                }

                _serverUrls = newServerAddrList;
                _lastSrvRefTime = DateTimeOffset.Now.ToUnixTimeSeconds();

                _serverUrls = new List<string>(newServerAddrList);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "failed to update server list");
            }
        }

        private async Task<List<string>> GetServerListFromEndpointAsync()
        {
            var list = new List<string>();
            try
            {
                var url = $"http://{_endpoint}/nacos/serverlist";

                var client = _clientFactory.CreateClient();
                client.Timeout = TimeSpan.FromMilliseconds(3000);

                var req = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, url);
                req.Headers.TryAddWithoutValidation("Client-Version", ConstValue.ClientVersion);
                req.Headers.TryAddWithoutValidation("User-Agent", ConstValue.ClientVersion);
                req.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip,deflate,sdch");
                req.Headers.TryAddWithoutValidation("RequestId", Guid.NewGuid().ToString());
                req.Headers.TryAddWithoutValidation("Request-Module", ConstValue.RequestModule);

                var resp = await client.SendAsync(req);

                if (!resp.IsSuccessStatusCode)
                {
                    throw new Exception($"Error while requesting: {url} . Server returned: {resp.StatusCode}");   
                }

                var str = await resp.Content.ReadAsStringAsync();
                using (StringReader sr = new StringReader(str))
                {
                    while (true)
                    {
                        var line = await sr.ReadLineAsync();
                        if (line == null || line.Length <= 0)
                            break;

                        list.Add(line.Trim());
                    }
                }
               
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error GetServerListFromEndpointAsync");                
                return null;
            }
        }

        public async Task<string> ReqApiAsync(HttpMethod httpMethod, string path, Dictionary<string, string> headers, Dictionary<string, string> paramValues, int timeout)
        {
            if ((_serverUrls == null || !_serverUrls.Any())
                && string.IsNullOrWhiteSpace(_nacosDomain))
                throw new Nacos.Exceptions.NacosException(ConstValue.CLIENT_INVALID_PARAM, "no server available");

            if (_serverUrls != null && _serverUrls.Any())
            {
                Random random = new Random();
                int index = random.Next(_serverUrls.Count);

                for (int i = 0; i < _serverUrls.Count; i++)
                {
                    var server = _serverUrls[index];
                    try
                    {
                        return await CallServerAsync(httpMethod, server, path, headers, paramValues, timeout);
                    }
                    catch (Nacos.Exceptions.NacosException ex)
                    {
                        _logger.LogDebug(ex, "request {0} failed.", server);
                    }

                    index = (index + 1) % _serverUrls.Count;
                }
            }

            if (!string.IsNullOrWhiteSpace(_nacosDomain))
            {
                for (int i = 0; i < ConstValue.REQUEST_DOMAIN_RETRY_COUNT; i++)
                {
                    try
                    {
                        return await CallServerAsync(httpMethod, _nacosDomain, path, headers, paramValues, timeout);
                    }
                    catch (Nacos.Exceptions.NacosException ex)
                    {
                        _logger.LogDebug(ex, "request {0} failed.", _nacosDomain);
                    }
                }
            }

            _logger.LogError("request: {0} failed, servers: {1}", path, string.Join(",", _serverUrls));

            throw new Nacos.Exceptions.NacosException(0, $"failed to req API:{path} after all servers(" + string.Join(",", _serverUrls) + ") tried: ");
        }

        private async Task<string> CallServerAsync(HttpMethod httpMethod, string server, string path, Dictionary<string, string> headers, Dictionary<string, string> paramValues, int timeout)
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = httpMethod
            };

            CheckSignature(paramValues);

            requestMessage.Headers.TryAddWithoutValidation("Client-Version", ConstValue.ClientVersion);
            requestMessage.Headers.TryAddWithoutValidation("User-Agent", ConstValue.ClientVersion);
            requestMessage.Headers.TryAddWithoutValidation("RequestId", Guid.NewGuid().ToString());
            requestMessage.Headers.TryAddWithoutValidation("Request-Module", ConstValue.RequestModule);

            var client = _clientFactory.CreateClient(ConstValue.ClientName);
            client.Timeout = TimeSpan.FromMilliseconds(timeout);
           
            var requestUrl = string.Empty;
            if (server.StartsWith(ConstValue.HTTPS, StringComparison.OrdinalIgnoreCase)
                || server.StartsWith(ConstValue.HTTP, StringComparison.OrdinalIgnoreCase))
            {
                requestUrl = $"{server}{path}";
            }
            else 
            {
                if (server.Contains(ConstValue.SERVER_ADDR_IP_SPLITER))
                {
                    server = server + ConstValue.SERVER_ADDR_IP_SPLITER + _serverPort;
                }

                requestUrl = $"http://{server}{path}";
            }

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

            var responseMessage = await client.SendAsync(requestMessage);

            if (responseMessage.IsSuccessStatusCode)
            {
                var content = await responseMessage.Content.ReadAsStringAsync();
                return content;
            }
            if (responseMessage.StatusCode == System.Net.HttpStatusCode.NotModified)
            {
                return string.Empty;
            }

            throw new Nacos.Exceptions.NacosException((int)responseMessage.StatusCode, "");
        }

        private void CheckSignature(Dictionary<string, string> param)
        {
            param.Add("app", AppDomain.CurrentDomain.FriendlyName);
            if (string.IsNullOrWhiteSpace(_options.AccessKey) 
                && string.IsNullOrWhiteSpace(_options.SecretKey))
                return;

            string signData = string.IsNullOrWhiteSpace(param["serviceName"]) 
                ? DateTimeOffset.Now.ToUnixTimeSeconds().ToString() + "@@" + param["serviceName"] 
                : DateTimeOffset.Now.ToUnixTimeSeconds().ToString();

            string signature = Utilities.HashUtil.GetHMACSHA1(signData, _options.SecretKey);
            param.Add("signature", signature);
            param.Add("data", signData);
            param.Add("ak", _options.AccessKey);
        }

        public void Dispose()
        {
            t?.Dispose();
        }
    }
}

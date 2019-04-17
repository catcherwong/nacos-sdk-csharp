namespace Nacos
{
    using Microsoft.Extensions.Options;
    using Nacos.Exceptions;
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public partial class NacosClient : INacosClient
    {
        private readonly NacosOptions _options;
        private readonly IHttpClientFactory _clientFactory;
        private readonly Timer _timer;
        private bool _polling;

        public NacosClient(IOptionsMonitor<NacosOptions> optionAccs, IHttpClientFactory clientFactory)
        {
            this._options = optionAccs.CurrentValue;
            this._clientFactory = clientFactory;

            if (_options.EnablePollingConfig)
            {
                _timer = new Timer(x =>
                {
                    if (_polling)
                    {
                        return;
                    }

                    _polling = true;
                    Polling();
                    _polling = false;
                }, null, 1000, 1000);
            }
        }

        private void Polling()
        {

        }

        public async Task<string> GetConfigAsync(GetConfigRequest request)
        {
            if (!request.IsValid())
            {
                throw new RequestInValidException("request 参数不合法");
            }

            var responseMessage = await DoRequestAsync(HttpMethod.Get, "/nacos/v1/cs/configs", request.ToQueryString());

            if (responseMessage.IsSuccessStatusCode)
            {
                var result = await responseMessage.Content.ReadAsStringAsync();
                return result;
            }

            switch (responseMessage.StatusCode)
            {
                case System.Net.HttpStatusCode.BadRequest:
                    return null;
                case System.Net.HttpStatusCode.Forbidden:
                    return null;
                case System.Net.HttpStatusCode.NotFound:
                    return null;
                case System.Net.HttpStatusCode.InternalServerError:
                    return null;
                default:
                    throw new Exception();
            }
        }

        public async Task<bool> PublishConfigAsync(PublishConfigRequest request)
        {
            if (!request.IsValid())
            {
                throw new RequestInValidException("request 参数不合法");
            }

            var responseMessage = await DoRequestAsync(HttpMethod.Post, "/nacos/v1/cs/configs", request.ToQueryString());

            if (responseMessage.IsSuccessStatusCode)
            {
                var result = await responseMessage.Content.ReadAsStringAsync();
                return result.Equals("true", StringComparison.OrdinalIgnoreCase);
            }

            switch (responseMessage.StatusCode)
            {
                case System.Net.HttpStatusCode.BadRequest:
                    return false;
                case System.Net.HttpStatusCode.Forbidden:
                    return false;
                case System.Net.HttpStatusCode.NotFound:
                    return false;
                case System.Net.HttpStatusCode.InternalServerError:
                    return false;
                default:
                    throw new Exception();
            }
        }

        public async Task<bool> RemoveConfigAsync(RemoveConfigRequest request)
        {
            if (!request.IsValid())
            {
                throw new RequestInValidException("request 参数不合法");
            }

            var responseMessage = await DoRequestAsync(HttpMethod.Delete, "/nacos/v1/cs/configs", request.ToQueryString());

            if (responseMessage.IsSuccessStatusCode)
            {
                var result = await responseMessage.Content.ReadAsStringAsync();
                return result.Equals("true", StringComparison.OrdinalIgnoreCase);
            }

            switch (responseMessage.StatusCode)
            {
                case System.Net.HttpStatusCode.BadRequest:
                    return false;
                case System.Net.HttpStatusCode.Forbidden:
                    return false;
                case System.Net.HttpStatusCode.NotFound:
                    return false;
                case System.Net.HttpStatusCode.InternalServerError:
                    return false;
                default:
                    throw new Exception();
            }
        }
    }
}

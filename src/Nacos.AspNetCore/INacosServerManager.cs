namespace Nacos.AspNetCore
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using EasyCaching.Core;

    public interface INacosServerManager
    {
        Task<string> GetServerAsync(string serviceName);
    }

    public class NacosServer
    {
        public string Url { get; set; }
    }

    public class NacosServerManager : INacosServerManager
    {
        private readonly INacosNamingClient _client;

        private readonly IEasyCachingProvider _provider;

        public NacosServerManager(INacosNamingClient client, IEasyCachingProviderFactory factory)
        {
            _client = client;
            _provider = factory.GetCachingProvider("nacos.aspnetcore");
        }

        public async Task<string> GetServerAsync(string serviceName)
        {
            var cached = await _provider.GetAsync(serviceName, async () =>
            {
                var serviceInstances = await _client.ListInstancesAsync(new ListInstancesRequest
                {
                    ServiceName = serviceName
                });

                var baseUrl = string.Empty;

                if (serviceInstances != null && serviceInstances.Hosts != null && serviceInstances.Hosts.Any())
                {
                    var list = serviceInstances.Hosts.Select(x => new NacosServer
                    {
                        Url = $"http://{x.Ip}:{x.Port}"
                    }).ToList();

                    return list;
                }

                return null;
            }, TimeSpan.FromMinutes(1));

            if (cached.HasValue)
            {
                var list = cached.Value;
                var index = new Random().Next(0, list.Count);
                return list[index].Url;
            }
            else
            {
                return null;
            }
        }
    }
}

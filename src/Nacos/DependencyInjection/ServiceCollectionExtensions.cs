namespace Nacos
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using System;
    using System.Net.Http;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNacos(this IServiceCollection services, Action<NacosOptions> configure)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
          
            services.AddOptions();
            services.Configure(configure);

            services.AddHttpClient(ConstValue.ClientName)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler() { UseProxy = false });

            services.TryAddSingleton<ILocalConfigInfoProcessor, MemoryLocalConfigInfoProcessor>();
            services.AddSingleton<INacosClient, NacosClient>();

            return services;
        }

        public static IServiceCollection AddNacos(this IServiceCollection services, Action<NacosOptions> configure, Action<HttpClient> httpClientAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions();
            services.Configure(configure);

            services.AddHttpClient(ConstValue.ClientName)
                .ConfigureHttpClient(httpClientAction)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler() { UseProxy = false });

            services.TryAddSingleton<ILocalConfigInfoProcessor, MemoryLocalConfigInfoProcessor>();
            services.AddSingleton<INacosClient, NacosClient>();

            return services;
        }
    }
}

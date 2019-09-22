namespace Microsoft.Extensions.DependencyInjection
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Nacos;
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
            services.AddSingleton<INacosConfigClient, NacosConfigClient>();
            services.AddSingleton<INacosNamingClient, NacosNamingClient>();

            return services;
        }

        public static IServiceCollection AddNacos(this IServiceCollection services, IConfiguration configuration, string sectionName = "nacos")
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.Configure<NacosOptions>(configuration.GetSection(sectionName));            

            services.AddHttpClient(ConstValue.ClientName)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler() { UseProxy = false });
                
            services.TryAddSingleton<ILocalConfigInfoProcessor, MemoryLocalConfigInfoProcessor>();
            services.AddSingleton<INacosConfigClient, NacosConfigClient>();
            services.AddSingleton<INacosNamingClient, NacosNamingClient>();

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
            services.AddSingleton<INacosConfigClient, NacosConfigClient>();
            services.AddSingleton<INacosNamingClient, NacosNamingClient>();

            return services;
        }

        public static IServiceCollection AddNacos(this IServiceCollection services, IConfiguration configuration, Action<HttpClient> httpClientAction, string sectionName = "nacos")
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.Configure<NacosOptions>(configuration.GetSection(sectionName));   

            services.AddHttpClient(ConstValue.ClientName)
                .ConfigureHttpClient(httpClientAction)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler() { UseProxy = false });

            services.TryAddSingleton<ILocalConfigInfoProcessor, MemoryLocalConfigInfoProcessor>();
            services.AddSingleton<INacosConfigClient, NacosConfigClient>();
            services.AddSingleton<INacosNamingClient, NacosNamingClient>();

            return services;
        }
    }
}

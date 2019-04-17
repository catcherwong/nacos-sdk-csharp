namespace Microsoft.Extensions.DependencyInjection
{
    using Nacos;
    using System;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNacos(this IServiceCollection services, Action<NacosOptions> configure)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddHttpClient(ConstValue.ClientName);

            services.AddOptions();
            services.Configure(configure);

            services.AddSingleton<INacosClient, NacosClient>();

            return services;
        }
    }
}

namespace Microsoft.AspNetCore.Builder
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Nacos.AspNetCore;

    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add Nacos AspNetCore.
        /// </summary>
        /// <param name="services">services.</param>
        /// <param name="configuration">configuration</param>
        /// <returns></returns>
        public static IServiceCollection AddNacosAspNetCore(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<NacosAspNetCoreOptions>(configuration.GetSection("nacos"));

            //services.AddNacos(configuration);
            services.AddNacosNaming(configuration);

            services.AddEasyCaching(options =>
            {
                options.UseInMemory("nacos.aspnetcore");
            });

            services.AddSingleton<INacosServerManager, NacosServerManager>();

            // IHostedService, report instance status
            services.AddHostedService<StatusReportBgTask>();

            return services;
        }
    }
}

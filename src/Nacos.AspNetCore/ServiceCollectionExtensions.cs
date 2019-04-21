namespace Nacos.AspNetCore
{
    using System;
    using System.Linq;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Hosting.Server.Features;
    using Microsoft.AspNetCore.Http.Features;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using EasyCaching.InMemory;
    using EasyCaching.Core;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNacosAspNetCore(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<NacosAspNetCoreOptions>(configuration.GetSection("nacos"));

            services.AddNacos(configuration);

            services.AddEasyCaching(options=> 
            {
                options.UseInMemory("nacos.aspnetcore");
            });

            services.AddSingleton<INacosServerManager, NacosServerManager>();

            return services;
        }

        public static IApplicationBuilder UseNacosAspNetCore(this IApplicationBuilder app)
        {
            var namingClient = app.ApplicationServices.GetRequiredService<INacosNamingClient>();
            var nacosAspNetCoreConfig = app.ApplicationServices.GetRequiredService<IOptions<NacosAspNetCoreOptions>>();
            var logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>().CreateLogger("Nacos.AspNetCore");
            var lifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();

            if (!(app.Properties["server.Features"] is FeatureCollection features)) return app;

            var addresses = features.Get<IServerAddressesFeature>();
            var address = addresses.Addresses.First();

            var uri = new Uri(address);
            
            var registrerRequest = new RegisterInstanceRequest
            {
                ServiceName = nacosAspNetCoreConfig.Value.ServiceName,
                Ip = uri.Host,
                Port = uri.Port,
                GroupName = nacosAspNetCoreConfig.Value.GroupName,
                NamespaceId = nacosAspNetCoreConfig.Value.Namespace,
                ClusterName = nacosAspNetCoreConfig.Value.ClusterName,
                Enable = true,
                Ephemeral = false
            };

            namingClient.RegisterInstanceAsync(registrerRequest).ConfigureAwait(true);

            lifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation("Unregistering from Nacos");

                var removeRequest = new RemoveInstanceRequest
                {
                    ServiceName = nacosAspNetCoreConfig.Value.ServiceName,
                    Ip = uri.Host,
                    Port = uri.Port,
                    GroupName = nacosAspNetCoreConfig.Value.GroupName,
                    NamespaceId = nacosAspNetCoreConfig.Value.Namespace,
                    ClusterName = nacosAspNetCoreConfig.Value.ClusterName,
                    Ephemeral = false
                };

                namingClient.RemoveInstanceAsync(removeRequest).ConfigureAwait(true);
            });

            return app;
        }
    }
}

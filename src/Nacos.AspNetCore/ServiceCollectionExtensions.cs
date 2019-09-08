namespace Nacos.AspNetCore
{
    using EasyCaching.Core;
    using EasyCaching.InMemory;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Hosting.Server.Features;
    using Microsoft.AspNetCore.Http.Features;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNacosAspNetCore(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<NacosAspNetCoreOptions>(configuration.GetSection("nacos"));

            services.AddNacos(configuration);

            services.AddEasyCaching(options =>
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

            //var registrerRequest = new RegisterInstanceRequest
            //{
            //    ServiceName = nacosAspNetCoreConfig.Value.ServiceName,
            //    Ip = uri.Host,
            //    Port = uri.Port,
            //    GroupName = nacosAspNetCoreConfig.Value.GroupName,
            //    NamespaceId = nacosAspNetCoreConfig.Value.Namespace,
            //    ClusterName = nacosAspNetCoreConfig.Value.ClusterName,
            //    Enable = true,
            //    Ephemeral = false,
            //};

            var timer = new Timer(async x =>
            {
                await SendAsync(namingClient, nacosAspNetCoreConfig.Value, uri, logger);
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));

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

        private static async Task SendAsync(INacosNamingClient client, NacosAspNetCoreOptions options, Uri uri, ILogger logger)
        {
            try
            {
                // send heart beat will register instance
                await client.SendHeartbeatAsync(new SendHeartbeatRequest
                {
                    Ephemeral = false,
                    ServiceName = options.ServiceName,
                    GroupName = options.GroupName,
                    BeatInfo = new BeatInfo
                    {
                        ip = uri.Host,
                        port = uri.Port,
                        serviceName = options.ServiceName,
                        scheduled = true,
                        weight = options.Weight,
                        cluster = options.ClusterName,
                    },
                });
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Send heart beat to Nacos error");
            }
        }
    }
}

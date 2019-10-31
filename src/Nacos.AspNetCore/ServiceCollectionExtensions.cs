namespace Microsoft.AspNetCore.Builder
{
    using Nacos;
    using Nacos.AspNetCore;
    using Microsoft.AspNetCore.Hosting.Server.Features;
    using Microsoft.AspNetCore.Http.Features;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
#if NETCORE3
    using Microsoft.Extensions.Hosting;
#else
    using EasyCaching.InMemory;
    using EasyCaching.Core;
    using Microsoft.AspNetCore.Hosting;
#endif
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

            if (!(app.Properties["server.Features"] is FeatureCollection features)) return app;

            var addresses = features.Get<IServerAddressesFeature>();
            var address = addresses.Addresses.First();

            var uri = new Uri(address);

            var timer = new Timer(async x =>
            {
                await SendAsync(namingClient, nacosAspNetCoreConfig.Value, uri, logger);
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));

#if !NETCORE3
            var lifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();
           
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

                timer.Change(Timeout.Infinite, 0);
                timer.Dispose();
            });
#else
            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

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

                timer.Change(Timeout.Infinite, 0);
                timer.Dispose();
            });
#endif

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
                     NameSpaceId = options.Namespace
                });
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Send heart beat to Nacos error");
            }
        }
    }
}

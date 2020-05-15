namespace Microsoft.AspNetCore.Builder
{
    using Nacos;
    using Nacos.AspNetCore;
    using Microsoft.AspNetCore.Hosting.Server.Features;
    using Microsoft.AspNetCore.Http.Features;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Net;

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

            var uri = GetUri(app, nacosAspNetCoreConfig.Value);

            var timer = new Timer(async x =>
            {
                await SendAsync(namingClient, nacosAspNetCoreConfig.Value, uri, logger);
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));

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

        private static Uri GetUri(IApplicationBuilder app, NacosAspNetCoreOptions config)
        {
            Uri uri = null;

            var port = config.Port <= 0 ? 80 : config.Port;

            // config first
            if (!string.IsNullOrWhiteSpace(config.Ip))
            {
                // it seems that nacos don't return the scheme
                // so here use http only.
                return new Uri($"http://{config.Ip}:{port}");
            }

            var address = string.Empty;

            // IServerAddressesFeature second
            if (app.Properties["server.Features"] is FeatureCollection features)
            {
                var addresses = features.Get<IServerAddressesFeature>();
                address = addresses?.Addresses?.First();

                if (address != null)
                {
                    if (address.Contains("*"))
                    {
                        var ip = GetCurrentIp();

                        address = address.Replace("*", ip);
                    }

                    uri = new Uri(address);
                    return uri;
                }
            }

            // current ip address third
            address = $"{config.Scheme}://{GetCurrentIp()}:{port}";

            uri = new Uri(address);
            return uri;
        }

        private static string GetCurrentIp()
        {
            var instanceIp = "127.0.0.1";

            try
            {
                IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());

                foreach (var ipAddr in Dns.GetHostAddresses(Dns.GetHostName()))
                {
                    if (ipAddr.AddressFamily.ToString() == "InterNetwork")
                    {
                        instanceIp = ipAddr.ToString();
                        break;
                    }
                }
            }
            catch
            {
            }

            return instanceIp;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MsConfigApp
{
    public class Program
    {
        //private static IConfiguration configuration = new ConfigurationBuilder()
        //   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        //   .Build();

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                 .ConfigureAppConfiguration((context, builder) =>
                 {
                     builder.AddNacosConfiguration(x =>
                     {
                         x.DataId = "msconfigapp";
                         x.Group = "";
                         x.Tenant = "f47e0ae1-982a-4a64-aea3-52506492a3d4";
                         x.Optional = false;
                         x.NacosConfigClient = null;
                     });
                 })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

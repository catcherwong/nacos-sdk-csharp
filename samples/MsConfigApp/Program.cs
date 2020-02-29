using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace MsConfigApp
{
    public class Program
    {
        //private static IConfiguration configuration = new ConfigurationBuilder()
        //   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        //   .Build();

        public static void Main(string[] args)
        {
            var outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}";

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Debug()
                .WriteTo.Console(
                    outputTemplate: outputTemplate)
                /*.WriteTo.File(
                    path: "logs/ApiTpl.log",
                    outputTemplate: outputTemplate,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 5,
                    encoding: System.Text.Encoding.UTF8)*/
                .CreateLogger();

            try
            {
                Log.ForContext<Program>().Information("Application starting...");
                CreateHostBuilder(args).Build().Run();
            }
            catch (System.Exception ex)
            {
                Log.ForContext<Program>().Fatal(ex, "Application start-up failed!!");
            }
            finally
            {
                Log.CloseAndFlush();
            }
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
                         x.ServerAddresses = new List<string> { "localhost:8848" };
                     });
                 })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
            .UseSerilog();
    }
}

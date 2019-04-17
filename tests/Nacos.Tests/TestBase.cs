using System;
namespace Nacos.Tests
{
    using Microsoft.Extensions.DependencyInjection;
    using Nacos;

    public class TestBase
    {
        protected INacosClient _client;

        public TestBase()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddNacos(configure =>
            {
                configure.DefaultTimeOut = 8;
                configure.EnablePollingConfig = false;
                configure.EndPoint = "http://127.0.0.1:8848";
            });

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            _client = serviceProvider.GetService<INacosClient>();
        }
    }
}

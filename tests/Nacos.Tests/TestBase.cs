namespace Nacos.Tests
{
    using Microsoft.Extensions.DependencyInjection;
    using Nacos;
    using System;

    public class TestBase
    {
        protected INacosClient _client;

        public TestBase()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddNacos(configure =>
            {
                configure.DefaultTimeOut = 8;                
                configure.EndPoint = "http://192.168.12.209:8848";
            });

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            _client = serviceProvider.GetService<INacosClient>();
        }
    }
}

﻿namespace Nacos.Tests
{
    using Microsoft.Extensions.DependencyInjection;
    using Nacos;
    using System;

    public class TestBase
    {
        protected INacosNamingClient _namingClient;
        protected INacosConfigClient _configClient;

        public TestBase()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddNacos(configure =>
            {
                configure.DefaultTimeOut = 8;
                configure.ServerAddresses = new System.Collections.Generic.List<string> { "http://localhost:8848", };
                configure.AccessKey = "";
                configure.SecretKey = "";
                configure.Namespace = "";
            });

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            _namingClient = serviceProvider.GetService<INacosNamingClient>();
            _configClient = serviceProvider.GetService<INacosConfigClient>();
        }
    }
}

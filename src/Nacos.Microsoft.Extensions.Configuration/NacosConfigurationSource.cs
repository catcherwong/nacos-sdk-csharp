namespace Nacos.Microsoft.Extensions.Configuration
{
    using global::Microsoft.Extensions.Configuration;
    using System.Collections.Generic;

    public class NacosConfigurationSource : IConfigurationSource
    {
        public List<string> ServerAddresses { get; set; }

        public bool Optional { get; set; }

        public string DataId { get; set; }

        public string  Group { get; set; }

        public string Tenant { get; set; }

        public INacosConfigurationParser NacosConfigurationParser { get; set; }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new NacosConfigurationProvider(this);
        }
    }
}

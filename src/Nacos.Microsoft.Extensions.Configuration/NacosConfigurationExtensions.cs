namespace Microsoft.Extensions.Configuration
{
    using Nacos.Microsoft.Extensions.Configuration;
    using System;

    public static class NacosConfigurationExtensions
    {
        public static IConfigurationBuilder AddNacosConfiguration(
           this IConfigurationBuilder builder, Action<NacosConfigurationSource> action)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var source = new NacosConfigurationSource();

            action(source);

            return builder.Add(source);
        }
    }
}

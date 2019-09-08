namespace Nacos.AspNetCore
{
    using System.Collections.Generic;

    public class NacosAspNetCoreOptions
    {
        /// <summary>
        /// nacos server endpoint
        /// </summary>
        [System.Obsolete("replace with ServerAddresses")]
        public string EndPoint { get; set; } = "http://localhost:8848";

        /// <summary>
        /// nacos server addresses.
        /// </summary>
        /// <example>
        /// 10.1.12.123:8848,10.1.12.124:8848
        /// </example>
        public List<string> ServerAddresses { get; set; }

        /// <summary>
        /// default timeout, unit is second.
        /// </summary>
        public int DefaultTimeOut { get; set; } = 15;

        /// <summary>
        /// default namespace
        /// </summary>
        public string Namespace { get; set; } = "";

        /// <summary>
        /// the name of the service.
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// the name of the cluster.
        /// </summary>
        /// <value>The name of the cluster.</value>
        public string ClusterName { get; set; }

        /// <summary>
        /// the name of the group.
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// the weight of this instance.
        /// </summary>
        public double Weight { get; set; } = 10;
    }
}

namespace Nacos.AspNetCore
{
    public class NacosAspNetCoreOptions
    {
        /// <summary>
        /// nacos server endpoint
        /// </summary>
        public string EndPoint { get; set; } = "http://localhost:8848";

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
    }
}

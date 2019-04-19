namespace Nacos
{
    using Nacos.Utilities;
    using System.Text;

    public class RemoveInstanceRequest : BaseRequest
    {
        /// <summary>
        /// IP of instance
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// Port of instance
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Service name
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// ID of namespace
        /// </summary>
        public string NamespaceId { get; set; }

        /// <summary>
        /// Cluster name
        /// </summary>
        public string ClusterName { get; set; }

        /// <summary>
        /// group name
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// if instance is ephemeral
        /// </summary>
        public bool? Ephemeral { get; set; }

        public override void CheckParam()
        {
            ParamUtil.CheckInstanceInfo(Ip, Port, ServiceName);
        }

        public override string ToQueryString()
        {
            var sb = new StringBuilder(1024);
            sb.Append($"ip={Ip}&port={Port}&serviceName={ServiceName}");

            if (!string.IsNullOrWhiteSpace(NamespaceId))
            {             
                sb.Append($"&namespaceId={NamespaceId}");
            }
          
            if (!string.IsNullOrWhiteSpace(ClusterName))
            {
                sb.Append($"&clusterName={ClusterName}");
            }

            if (!string.IsNullOrWhiteSpace(GroupName))
            {
                sb.Append($"&groupName={GroupName}");
            }

            if (Ephemeral.HasValue)
            {
                sb.Append($"&ephemeral={Ephemeral}");
            }      

            return sb.ToString();
        }
    }
}

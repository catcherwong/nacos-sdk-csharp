namespace Nacos
{
    using System.Text;
    using Nacos.Utilities;

    public class RegisterInstanceRequest : BaseRequest
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
        /// service name
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// ID of namespace
        /// </summary>
        public string NamespaceId { get; set; }

        /// <summary>
        /// Weight
        /// </summary>
        public double? Weight { get; set; }

        /// <summary>
        /// enabled or not
        /// </summary>
        public bool? Enable { get; set; }

        /// <summary>
        /// healthy or not
        /// </summary>
        public bool? Healthy { get; set; }

        /// <summary>
        /// extended information
        /// </summary>
        public string Metadata { get; set; }

        /// <summary>
        /// cluster name
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

            if (Weight.HasValue)
            {
                sb.Append($"&weight={Weight.Value}");
            }

            if (Healthy.HasValue)
            {
                sb.Append($"&healthy={Healthy}");
            }

            if (!string.IsNullOrWhiteSpace(Metadata))
            {
                sb.Append($"&metadata={Metadata}");
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

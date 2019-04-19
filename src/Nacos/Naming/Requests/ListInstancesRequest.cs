namespace Nacos
{
    using System.Text;
    using Nacos.Utilities;

    public class ListInstancesRequest : BaseRequest
    {
        /// <summary>
        /// Service name
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// ID of namespace
        /// </summary>
        public string NamespaceId { get; set; }

        /// <summary>
        /// Cluster name, splited by comma
        /// </summary>
        public string Clusters { get; set; }

        /// <summary>
        /// group name
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Return healthy instance or not
        /// </summary>
        public bool? HealthyOnly { get; set; }

        public override void CheckParam()
        {
            ParamUtil.CheckServiceName(ServiceName);             
        }

        public override string ToQueryString()
        {
            var sb = new StringBuilder(1024);
            sb.Append($"serviceName={ServiceName}");

            if (!string.IsNullOrWhiteSpace(NamespaceId))
            {             
                sb.Append($"&namespaceId={NamespaceId}");
            }
          
            if (!string.IsNullOrWhiteSpace(Clusters))
            {
                sb.Append($"&clusters={Clusters}");
            }

            if (!string.IsNullOrWhiteSpace(GroupName))
            {
                sb.Append($"&groupName={GroupName}");
            }

            if (HealthyOnly.HasValue)
            {
                sb.Append($"&healthyOnly={HealthyOnly.Value}");
            }      

            return sb.ToString();
        }
    }
}

namespace Nacos
{
    using Nacos.Utilities;
    using System.Text;

    public class ModifyInstanceHealthStatusRequest : BaseRequest
    {
        /// <summary>
        /// ip of instance
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// port of instance
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// service name
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// namespace id
        /// </summary>
        public string NamespaceId { get; set; }

        /// <summary>
        /// if healthy
        /// </summary>
        public bool Healthy { get; set; }

        /// <summary>
        /// cluster name
        /// </summary>
        public string ClusterName { get; set; }

        /// <summary>
        /// group name
        /// </summary>
        public string GroupName { get; set; }

        public override void CheckParam()
        {
            ParamUtil.CheckInstanceInfo(Ip, Port, ServiceName);
        }

        public override string ToQueryString()
        {
            var sb = new StringBuilder(1024);
            sb.Append($"ip={Ip}&port={Port}&serviceName={ServiceName}&healthy={Healthy}");

            if (!string.IsNullOrWhiteSpace(GroupName))
            {
                sb.Append($"&groupName={GroupName}");
            }

            if (!string.IsNullOrWhiteSpace(NamespaceId))
            {
                sb.Append($"&namespaceId={NamespaceId}");
            }

            if (!string.IsNullOrWhiteSpace(ClusterName))
            {
                sb.Append($"&clusterName={ClusterName}");
            }


            return sb.ToString();
        }
    }
}

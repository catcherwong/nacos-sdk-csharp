namespace Nacos
{
    using System.Text;

    public class ModifyInstanceHealthStatusRequest : BaseRequest
    {
        /// <summary>
        /// 服务实例IP
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// 服务实例port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 服务名
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// 命名空间ID
        /// </summary>
        public string NamespaceId { get; set; }

        /// <summary>
        /// 是否健康
        /// </summary>
        public bool Healthy { get; set; }

        /// <summary>
        /// 集群名
        /// </summary>
        public string ClusterName { get; set; }

        /// <summary>
        /// 分组名
        /// </summary>
        public string GroupName { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Ip) && Port > 0 && !string.IsNullOrWhiteSpace(ServiceName);
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

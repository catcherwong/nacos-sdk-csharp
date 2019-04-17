namespace Nacos
{
    using System.Text;

    public class RegisterInstanceRequest : BaseRequest
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
        /// 权重
        /// </summary>
        public double? Weight { get; set; }

        /// <summary>
        /// 是否上线
        /// </summary>
        public bool? Enable { get; set; }

        /// <summary>
        /// 是否健康
        /// </summary>
        public bool? Healthy { get; set; }

        /// <summary>
        /// 扩展信息
        /// </summary>
        public string Metadata { get; set; }

        /// <summary>
        /// 集群名
        /// </summary>
        public string ClusterName { get; set; }

        /// <summary>
        /// 分组名
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 是否临时实例
        /// </summary>
        public bool? Ephemeral { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Ip) && Port > 0 && !string.IsNullOrWhiteSpace(ServiceName);
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

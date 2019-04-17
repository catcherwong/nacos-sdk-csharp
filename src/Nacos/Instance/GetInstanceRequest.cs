namespace Nacos
{
    using System.Text;

    public class GetInstanceRequest : BaseRequest
    {
        /// <summary>
        /// 实例IP
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// 实例端口
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
        /// 是否只返回健康实例
        /// </summary>
        public bool? HealthyOnly { get; set; }

        /// <summary>
        /// 集群名称
        /// </summary>
        public string Cluster { get; set; }
        
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
          
            if (!string.IsNullOrWhiteSpace(Cluster))
            {
                sb.Append($"&cluster={Cluster}");
            }

            if (!string.IsNullOrWhiteSpace(GroupName))
            {
                sb.Append($"&groupName={GroupName}");
            }

            if (HealthyOnly.HasValue)
            {
                sb.Append($"&healthyOnly={HealthyOnly.Value}");
            }

            if (Ephemeral.HasValue)
            {
                sb.Append($"&ephemeral={Ephemeral.Value}");
            }

            return sb.ToString();
        }
    }
}

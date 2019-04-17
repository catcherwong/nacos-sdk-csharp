namespace Nacos
{
    using System.Text;

    public class ListInstancesRequest : BaseRequest
    {
        /// <summary>
        /// 服务名
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// 命名空间ID
        /// </summary>
        public string NamespaceId { get; set; }

        /// <summary>
        /// 集群名称,多个集群用逗号分隔
        /// </summary>
        public string Clusters { get; set; }

        /// <summary>
        /// 分组名
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 是否只返回健康实例
        /// </summary>
        public bool? HealthyOnly { get; set; }

        public override bool IsValid()
        {
            return string.IsNullOrWhiteSpace(ServiceName);
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

using System.Text;

namespace Nacos
{
    public class ModifyServiceRequest : BaseRequest
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
        /// 保护阈值,取值0到1,默认0
        /// </summary>
        public double? ProtectThreshold { get; set; }

        /// <summary>
        /// 访问策略,JSON格式字符串
        /// </summary>
        public string Selector { get; set; }

        /// <summary>
        /// 元数据
        /// </summary>
        public string Metadata { get; set; }

        /// <summary>
        /// 分组名
        /// </summary>
        public string GroupName { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(ServiceName);
        }

        public override string ToQueryString()
        {
            var sb = new StringBuilder(1024);
            sb.Append($"serviceName={ServiceName}");

            if (!string.IsNullOrWhiteSpace(NamespaceId))
            {             
                sb.Append($"&namespaceId={NamespaceId}");
            }
         
            if (!string.IsNullOrWhiteSpace(Metadata))
            {
                sb.Append($"&metadata={Metadata}");
            }

            if (!string.IsNullOrWhiteSpace(GroupName))
            {
                sb.Append($"&groupName={GroupName}");
            }

            if (!string.IsNullOrWhiteSpace(Selector))
            {
                sb.Append($"&selector={Selector}");
            }

            if (ProtectThreshold.HasValue)
            {
                sb.Append($"&protectThreshold={ProtectThreshold}");
            }      

            return sb.ToString();
        }
    }
}

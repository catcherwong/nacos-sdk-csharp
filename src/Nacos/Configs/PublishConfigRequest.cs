namespace Nacos
{
    using System.Text;

    public class PublishConfigRequest : BaseRequest
    {
        /// <summary>
        /// 租户信息，对应 Nacos 的命名空间字段
        /// </summary>
        public string Tenant { get; set; }

        /// <summary>
        /// 配置 ID
        /// </summary>
        public string DataId { get; set; }

        /// <summary>
        /// 配置分组
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// 配置内容
        /// </summary>
        public string Content { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(DataId) && !string.IsNullOrWhiteSpace(Content);
        }

        public override string ToQueryString()
        {
            var sb = new StringBuilder(2048);
            sb.Append($"dataId={DataId}&group={Group}&content={Content}");

            if (!string.IsNullOrWhiteSpace(Tenant))
            {             
                sb.Append($"&tenant={Tenant}");
            }                     

            return sb.ToString();
        }
    }
}

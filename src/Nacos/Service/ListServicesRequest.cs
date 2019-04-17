namespace Nacos
{
    using System.Text;

    public class ListServicesRequest : BaseRequest
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageNo { get; set; }

        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 命名空间ID
        /// </summary>
        public string NamespaceId { get; set; }

        /// <summary>
        /// 分组名
        /// </summary>
        public string GroupName { get; set; }

        public override bool IsValid()
        {
            return PageNo > 0 && PageSize > 0;
        }

        public override string ToQueryString()
        {
            var sb = new StringBuilder(1024);
            sb.Append($"pageNo={PageNo}&pageSize={PageSize}");

            if (!string.IsNullOrWhiteSpace(NamespaceId))
            {
                sb.Append($"&namespaceId={NamespaceId}");
            }

            if (!string.IsNullOrWhiteSpace(GroupName))
            {
                sb.Append($"&groupName={GroupName}");
            }

            return sb.ToString();
        }
    }
}

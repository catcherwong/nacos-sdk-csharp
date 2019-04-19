namespace Nacos
{
    using System.Text;

    public class ListServicesRequest : BaseRequest
    {
        /// <summary>
        /// current page number
        /// </summary>
        public int PageNo { get; set; }

        /// <summary>
        /// page size
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// namespace id
        /// </summary>
        public string NamespaceId { get; set; }

        /// <summary>
        /// group name
        /// </summary>
        public string GroupName { get; set; }

        public override void CheckParam()
        {
            //return PageNo > 0 && PageSize > 0;
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

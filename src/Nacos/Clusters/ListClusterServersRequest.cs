namespace Nacos
{
    public class ListClusterServersRequest : BaseRequest
    {
        /// <summary>
        /// 是否只返回健康Server节点
        /// </summary>
        public bool? Healthy { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string ToQueryString()
        {
            var sb = string.Empty;           

            if (Healthy.HasValue)
            {
                sb = $"&healthy={Healthy.Value}";
            }      

            return sb;
        }
    }
}

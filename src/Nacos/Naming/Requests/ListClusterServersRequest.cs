namespace Nacos
{
    public class ListClusterServersRequest : BaseRequest
    {
        /// <summary>
        /// if return healthy servers only
        /// </summary>
        public bool? Healthy { get; set; }

        public override void CheckParam()
        {
            //return true;
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

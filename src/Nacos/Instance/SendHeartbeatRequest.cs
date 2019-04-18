namespace Nacos
{
    using System.Text;
    using Nacos.Utilities;

    public class SendHeartbeatRequest : BaseRequest
    {
        /// <summary>
        /// 服务名
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// 实例心跳内容,JSON格式字符串
        /// </summary>
        public string Beat => BeatInfo.ToJsonString();

        /// <summary>
        /// Beat信息
        /// </summary>
        public BeatInfo BeatInfo { get; set; }

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
            return BeatInfo != null && !string.IsNullOrWhiteSpace(ServiceName);
        }

        public override string ToQueryString()
        {
            var sb = new StringBuilder(1024);
            sb.Append($"beat={Beat}&serviceName={ServiceName}");
           
            if (!string.IsNullOrWhiteSpace(GroupName))
            {
                sb.Append($"&groupName={GroupName}");
            }
        
            if (Ephemeral.HasValue)
            {
                sb.Append($"&ephemeral={Ephemeral.Value}");
            }

            return sb.ToString();
        }
    }
}

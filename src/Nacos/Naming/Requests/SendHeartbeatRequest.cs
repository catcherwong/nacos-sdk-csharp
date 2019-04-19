namespace Nacos
{
    using System.Text;
    using Nacos.Utilities;

    public class SendHeartbeatRequest : BaseRequest
    {
        /// <summary>
        /// Service Name
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// beat content
        /// </summary>
        public string Beat => BeatInfo.ToJsonString();

        /// <summary>
        /// beat info
        /// </summary>
        public BeatInfo BeatInfo { get; set; }

        /// <summary>
        /// group name
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// if instance is ephemeral
        /// </summary>
        public bool? Ephemeral { get; set; }

        public override void CheckParam()
        {
            //return BeatInfo != null && !string.IsNullOrWhiteSpace(ServiceName);
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

namespace Nacos
{
    using System.Text;

    public class ModifySwitchesRequest : BaseRequest
    {
        /// <summary>
        /// 开关名
        /// </summary>
        public string Entry { get; set; }

        /// <summary>
        /// 开关值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 是否只在本机生效,true表示本机生效,false表示集群生效
        /// </summary>
        public bool? Debug { get; set; }
       
        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Entry) && !string.IsNullOrWhiteSpace(Value);
        }

        public override string ToQueryString()
        {
            var sb = new StringBuilder(1024);
            sb.Append($"entry={Entry}&value={Value}");
            
            if (Debug.HasValue)
            {
                sb.Append($"&debug={Debug.Value}");
            }      

            return sb.ToString();
        }
    }
}

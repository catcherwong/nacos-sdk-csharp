namespace Nacos
{
    using Nacos.Utilities;

    public class ListenerConfigRequest : BaseRequest
    {
        /// <summary>
        /// 配置 ID
        /// </summary>
        public string DataId { get; set; }

        /// <summary>
        /// 配置分组
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// 配置内容 MD5 值
        /// </summary>
        public string ContentMD5 => Md5Util.GetMd5(Content);

        /// <summary>
        /// 租户信息，对应 Nacos 的命名空间字段(非必填)
        /// </summary>
        public string Tenant { get; set; }

        /// <summary>
        /// 配置内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 监听数据报文。格式为 dataId^2Group^2contentMD5^2tenant^1或者dataId^2Group^2contentMD5^1。     
        /// </summary>
        public string ListeningConfigs => string.IsNullOrWhiteSpace(Tenant)
            ? $"{DataId}{CharacterUtil.TwoEncode}{Group}{CharacterUtil.TwoEncode}{ContentMD5}{CharacterUtil.OneEncode}"
            : $"{DataId}{CharacterUtil.TwoEncode}{Group}{CharacterUtil.TwoEncode}{ContentMD5}{CharacterUtil.TwoEncode}{Tenant}{CharacterUtil.OneEncode}";


        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(DataId);            
        }

        public override string ToQueryString()
        {
            return $"Listening-Configs={ListeningConfigs}";
        }
    }
}

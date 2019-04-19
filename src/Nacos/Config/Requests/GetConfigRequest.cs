namespace Nacos
{
    using System.Text;
    using Nacos.Utilities;

    public class GetConfigRequest : BaseRequest
    {
        /// <summary>
        /// Tenant information. It corresponds to the Namespace field in Nacos.
        /// </summary>
        public string Tenant { get; set; }

        /// <summary>
        /// Configuration ID
        /// </summary>
        public string DataId { get; set; }

        /// <summary>
        /// Configuration group
        /// </summary>
        public string Group { get; set; }

        public override void CheckParam()
        {
            ParamUtil.CheckKeyParam(DataId, Group);
        }

        public override string ToQueryString()
        {
            var sb = new StringBuilder(100);
            sb.Append($"dataId={DataId}&group={Group}");

            if (!string.IsNullOrWhiteSpace(Tenant))
            {             
                sb.Append($"&tenant={Tenant}");
            }                     

            return sb.ToString();
        }
    }
}

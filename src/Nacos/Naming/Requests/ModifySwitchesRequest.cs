namespace Nacos
{
    using System.Text;

    public class ModifySwitchesRequest : BaseRequest
    {
        /// <summary>
        /// switch name
        /// </summary>
        public string Entry { get; set; }

        /// <summary>
        /// switch value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// if affect the local server, true means yes, false means no, default true
        /// </summary>
        public bool? Debug { get; set; }
       
        public override void CheckParam()
        {
            //return !string.IsNullOrWhiteSpace(Entry) && !string.IsNullOrWhiteSpace(Value);
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

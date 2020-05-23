﻿namespace Nacos
{
    using System.Collections.Generic;
    using System.Text;
    using Nacos.Utilities;

    public class PublishConfigRequest : BaseRequest
    {
        /// <summary>
        /// The tenant, corresponding to the namespace field of Nacos
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

        /// <summary>
        /// Configuration content
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Configuration type, options value text, json, xml, yaml, html, properties
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Configuration application
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// Configuration tags
        /// </summary>
        public string Tag { get; set; }

        public override void CheckParam()
        {
            ParamUtil.CheckParam(DataId, Group, Content);            
        }

        public override string ToQueryString()
        {
            var sb = new StringBuilder(2048);
            sb.Append($"dataId={DataId}&group={Group}&content={Content}");

            if (!string.IsNullOrWhiteSpace(Tenant))
            {             
                sb.Append($"&tenant={Tenant}");
            }      

            if(!string.IsNullOrWhiteSpace(Type))
            {
                sb.Append($"&type={Type}");
            }

            if (!string.IsNullOrWhiteSpace(AppName))
            {
                sb.Append($"&appName={AppName}");
            }

            if (!string.IsNullOrWhiteSpace(Tag))
            {
                sb.Append($"&tag={Tag}");
            }

            return sb.ToString();
        }

        public Dictionary<string, string> ToDict()
        {
            var dict = new Dictionary<string, string>
            {
                { "dataId", DataId },
                { "group", Group },
                { "content", Content }
            };

            if (!string.IsNullOrWhiteSpace(Tenant))
                dict.Add("tenant", Tenant);

            if (!string.IsNullOrWhiteSpace(Type))
                dict.Add("type", Type);

            if (!string.IsNullOrWhiteSpace(AppName))
                dict.Add("appName", AppName);

            if (!string.IsNullOrWhiteSpace(Tag))
                dict.Add("tag", Tag);

            return dict;
        }
    }
}

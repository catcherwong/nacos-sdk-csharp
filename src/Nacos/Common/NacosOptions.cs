namespace Nacos
{
    using System.Collections.Generic;

    public class NacosOptions
    {
        /// <summary>
        /// nacos server addresses.
        /// </summary>
        /// <example>
        /// http://10.1.12.123:8848,https://10.1.12.124:8848
        /// </example>
        public List<string> ServerAddresses { get; set; }

        /// <summary>
        /// default timeout, unit is second.
        /// </summary>
        public int DefaultTimeOut { get; set; } = 15;

        /// <summary>
        /// default namespace
        /// </summary>
        public string Namespace { get; set; } = "";

        /// <summary>
        /// accessKey
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// secretKey
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// listen interval, unit is millisecond.
        /// </summary>
        public int ListenInterval { get; set; } = 1000;
    }
}

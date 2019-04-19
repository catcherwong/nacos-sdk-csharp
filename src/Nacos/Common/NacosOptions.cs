namespace Nacos
{
    public class NacosOptions
    {
        /// <summary>
        /// nacos server endpoint
        /// </summary>
        public string EndPoint { get; set; } = "http://localhost:8848";
        
        /// <summary>
        /// default timeout
        /// </summary>
        public int DefaultTimeOut { get; set; } = 15;

        /// <summary>
        /// default namespace
        /// </summary>
        public string Namespace { get; set; } = "";
    }
}

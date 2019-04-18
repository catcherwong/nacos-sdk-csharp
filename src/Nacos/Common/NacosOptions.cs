namespace Nacos
{
    public class NacosOptions
    {
        public string EndPoint { get; set; } = "http://localhost:8848";
        
        public int DefaultTimeOut { get; set; } = 15;

        public string Namespace { get; set; } = "";
    }
}

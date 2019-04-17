namespace Nacos
{
    public class GetInstanceResult
    {
        public object Metadata { get; set; }
        public string InstanceId { get; set; }
        public int Port { get; set; }
        public string Service { get; set; }
        public bool Healthy { get; set; }

        public string Ip { get; set; }
        public string ClusterName { get; set; }
        public double Weight { get; set; }
    }
}

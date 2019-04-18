namespace Nacos
{
    using System.Collections.Generic;

    public class BeatInfo
    {
        public int Port { get; set; }
        public string Ip { get; set; }
        public double Weight { get; set; }
        public string ServiceName { get; set; }
        public string Cluster { get; set; }     
        public Dictionary<string,string> Metadata { get; set; }
        public bool Scheduled { get; set; }
    }
}

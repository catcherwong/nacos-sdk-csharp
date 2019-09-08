namespace Nacos
{
    using System.Collections.Generic;

    public class BeatInfo
    {
        public int port { get; set; }
        public string ip { get; set; }
        public double weight { get; set; }
        public string serviceName { get; set; }
        public string cluster { get; set; }
        public Dictionary<string, string> metadata { get; set; } = new Dictionary<string, string>();
        public bool scheduled { get; set; }
    }
}

﻿namespace Nacos
{
    using System.Collections.Generic;

    public class GetServiceResult
    {
        public object Metadata { get; set; }
        public string GroupName { get; set; }
        public string NamespaceId { get; set; }
        public string Name { get; set; }
        public Selector Selector { get; set; }
        public double ProtectThreshold { get; set; }
        public List<Cluster> Clusters { get; set; }
    }   
}

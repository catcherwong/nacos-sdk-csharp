namespace Nacos.AspNetCore
{
    using System.Collections.Generic;

    public class WeightRoundRobinLBStrategy : ILBStrategy
    {
        public LBStrategyName Name => LBStrategyName.WeightRoundRobin;

        public string GetInstance(List<NacosServer> list)
        {
            throw new System.NotImplementedException();
        }
    }
}

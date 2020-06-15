namespace Nacos.AspNetCore
{
    using System.Collections.Generic;

    public class WeightRandomLBStrategy : ILBStrategy
    {
        public LBStrategyName Name => LBStrategyName.WeightRandom;

        public string GetInstance(List<NacosServer> list)
        {
            throw new System.NotImplementedException();
        }
    }
}

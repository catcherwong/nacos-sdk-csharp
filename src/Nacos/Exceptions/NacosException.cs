namespace Nacos.Exceptions
{
    using System;

    public class NacosException : Exception
    {
        public NacosException(string message) : base(message)
        { }
    }
}

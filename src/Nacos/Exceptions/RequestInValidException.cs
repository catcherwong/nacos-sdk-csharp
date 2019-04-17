namespace Nacos.Exceptions
{
    using System;

    public class RequestInValidException : Exception
    {
        public RequestInValidException(string message) : base(message)
        { }
    }
}

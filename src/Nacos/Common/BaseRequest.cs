namespace Nacos
{
    public abstract class BaseRequest
    {        
        /// <summary>
        /// Convert request to params of API
        /// </summary>
        /// <returns></returns>
        public abstract string ToQueryString();

        /// <summary>
        /// Checks whether request is valid
        /// </summary>
        /// <returns></returns>
        public abstract void CheckParam();
    }
}

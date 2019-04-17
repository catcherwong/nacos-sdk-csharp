namespace Nacos
{
    public abstract class BaseRequest
    {
        /// <summary>
        /// 将Request转成参数
        /// </summary>
        /// <returns></returns>
        public abstract string ToQueryString();

        /// <summary>
        /// 检验参数是否合法
        /// </summary>
        /// <returns></returns>
        public abstract bool IsValid();
    }
}

namespace Nacos
{
    using System.Threading.Tasks;
 
    public interface INacosConfigClient
    {
        /// <summary>
        /// Gets configurations in Nacos
        /// </summary>
        /// <param name="request">request</param>        
        /// <returns></returns>        
        Task<string> GetConfigAsync(GetConfigRequest request);

        /// <summary>
        /// Publishes configurations in Nacos
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        Task<bool> PublishConfigAsync(PublishConfigRequest request);

        /// <summary>
        /// Deletes configurations in Nacos
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        Task<bool> RemoveConfigAsync(RemoveConfigRequest request);

        /// <summary>
        /// Listens for configurations in Nacos 
        /// </summary>        
        /// <param name="request">request</param>
        /// <returns></returns>
        Task ListenerConfigAsync(ListenerConfigRequest request);
    }
}

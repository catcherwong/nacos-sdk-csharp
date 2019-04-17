namespace Nacos
{
    using System.Threading.Tasks;

    public partial interface INacosClient
    {
        #region Config

        /// <summary>
        /// 获取Nacos上的配置
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        Task<string> GetConfigAsync(GetConfigRequest request);

        /// <summary>
        /// 发布 Nacos 上的配置
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        Task<bool> PublishConfigAsync(PublishConfigRequest request);

        /// <summary>
        /// 删除 Nacos 上的配置
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        Task<bool> RemoveConfigAsync(RemoveConfigRequest request);
        #endregion

        #region Instance
        /// <summary>
        /// 注册一个实例到服务
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        Task<bool> RegisterInstanceAsync(RegisterInstanceRequest request);

        /// <summary>
        /// 删除服务下的一个实例
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        Task<bool> RemoveInstanceAsync(RemoveInstanceRequest request);

        /// <summary>
        /// 修改服务下的一个实例
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        Task<bool> ModifyInstanceAsync(ModifyInstanceRequest request);

        /// <summary>
        /// 查询服务下的实例列表
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        Task<ListInstancesResult> ListInstancesAsync(ListInstancesRequest request);

        /// <summary>
        /// 查询一个服务下个某个实例详情
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        Task<GetInstanceResult> GetInstanceAsync(GetInstanceRequest request);

        /// <summary>
        /// 发送某个实例的心跳
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        Task<bool> SendHeartbeatAsync(SendHeartbeatRequest request);

        /// <summary>
        /// 更新实例的健康状态,仅在集群的健康检查关闭时才生效,当集群配置了健康检查时,该接口会返回错误
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        Task<bool> ModifyInstanceHealthStatusAsync(ModifyInstanceHealthStatusRequest request);
        #endregion

        #region Service
        /// <summary>
        /// 创建一个服务
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        Task<bool> CreateServiceAsync(CreateServiceRequest request);

        /// <summary>
        /// 删除一个服务,只有当服务下实例数为0时允许删除
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        Task<bool> RemoveServiceAsync(RemoveServiceRequest request);

        /// <summary>
        /// 更新一个服务
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        Task<bool> ModifyServiceAsync(ModifyServiceRequest request);

        /// <summary>
        /// 查询一个服务
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        Task<GetServiceResult> GetServiceAsync(GetServiceRequest request);

        /// <summary>
        /// 查询服务列表
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        Task<ListServicesResult> ListServicesAsync(ListServicesRequest request);
        #endregion

        #region Switches
        /// <summary>
        /// 查询系统开关
        /// </summary>
        /// <returns></returns>
        Task<GetSwitchesResult> GetSwitchesAsync();

        /// <summary>
        /// 修改系统开关
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        Task<bool> ModifySwitchesAsync(ModifySwitchesRequest request);
        #endregion

        #region Cluster
        /// <summary>
        /// 查看当前集群Server列表
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        Task<ListClusterServersResult> ListClusterServersAsync(ListClusterServersRequest request);

        /// <summary>
        /// 查看当前集群leader
        /// </summary>
        /// <returns></returns>
        Task<GetCurrentClusterLeaderResult> GetCurrentClusterLeaderAsync();
        #endregion

        #region Metrics
        /// <summary>
        /// 查看系统当前数据指标
        /// </summary>
        /// <returns></returns>
        Task<GetMetricsResult> GetMetricsAsync();
        #endregion
    }
}

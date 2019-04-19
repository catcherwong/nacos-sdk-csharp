﻿namespace Nacos
{
    using System.Threading.Tasks;
 
    public interface INacosNamingClient
    {
        #region Instance
        /// <summary>
        /// Register an instance to service
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        Task<bool> RegisterInstanceAsync(RegisterInstanceRequest request);

        /// <summary>
        /// Delete instance from service
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        Task<bool> RemoveInstanceAsync(RemoveInstanceRequest request);

        /// <summary>
        /// Modify an instance of service
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        Task<bool> ModifyInstanceAsync(ModifyInstanceRequest request);

        /// <summary>
        /// Query instance list of service
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        Task<ListInstancesResult> ListInstancesAsync(ListInstancesRequest request);

        /// <summary>
        /// Query instance details of service
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        Task<GetInstanceResult> GetInstanceAsync(GetInstanceRequest request);

        /// <summary>
        /// Send instance beat
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        Task<bool> SendHeartbeatAsync(SendHeartbeatRequest request);

        /// <summary>
        /// Update instance health status, only works when the cluster health checker is set to NONE
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        Task<bool> ModifyInstanceHealthStatusAsync(ModifyInstanceHealthStatusRequest request);
        #endregion

        #region Service
        /// <summary>
        /// Create service
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        Task<bool> CreateServiceAsync(CreateServiceRequest request);

        /// <summary>
        /// Delete a service, only permitted when instance count is 0
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        Task<bool> RemoveServiceAsync(RemoveServiceRequest request);

        /// <summary>
        /// Update a service
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        Task<bool> ModifyServiceAsync(ModifyServiceRequest request);

        /// <summary>
        /// Query a service
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        Task<GetServiceResult> GetServiceAsync(GetServiceRequest request);

        /// <summary>
        /// Query service list
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        Task<ListServicesResult> ListServicesAsync(ListServicesRequest request);
        #endregion

        #region Switches
        /// <summary>
        /// Query system switches
        /// </summary>
        /// <returns></returns>
        Task<GetSwitchesResult> GetSwitchesAsync();

        /// <summary>
        /// Update system switch
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        Task<bool> ModifySwitchesAsync(ModifySwitchesRequest request);
        #endregion

        #region Cluster
        /// <summary>
        /// Query server list
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        Task<ListClusterServersResult> ListClusterServersAsync(ListClusterServersRequest request);

        /// <summary>
        /// Query the leader of current cluster
        /// </summary>
        /// <returns></returns>
        Task<GetCurrentClusterLeaderResult> GetCurrentClusterLeaderAsync();
        #endregion

        #region Metrics
        /// <summary>
        /// Query system metrics
        /// </summary>
        /// <returns></returns>
        Task<GetMetricsResult> GetMetricsAsync();
        #endregion
    }
}

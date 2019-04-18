# nacos-sdk-csharp 　　　　　　　　　　[中文](./README.zh-cn.md)

Unofficial csharp(dotnet core) implementation of [nacos](https://nacos.io/) OpenAPI.

![](https://img.shields.io/nuget/v/nacos-sdk-csharp-unofficial.svg)

## Installation

```bash
dotnet add package nacos-sdk-csharp-unofficial
```

## Usages

### Dependency Injection(DI)

```cs
public class Startup
{
    //...
    
    public void ConfigureServices(IServiceCollection services)
    {
        // configuration
        services.AddNacos(configure =>
        {
            configure.DefaultTimeOut = 8;
            configure.EndPoint = "http://192.168.12.209:8848";
            configure.Namespace = "";
        });   

        //// or read from configuration file
        //services.AddNacos(Configuration);
    }    
}
```

Sample of configuration file

```JSON
{
    "nacos": {
        "EndPoint": "http://localhost:8848",
        "DefaultTimeOut": 15,
        "Namespace": ""
    }
}
```

### INacosClient

`INacosClient` is the entry of all opreations.

### Configuration Management

```cs

// Get configurations
var getConfigResult = await _client.GetConfigAsync(new GetConfigRequest
{
    DataId = "dataId",
    Group = "DEFAULT_GROUP",
    //Tenant = "tenant"
});

// Publish configuration
var publishConfigRessult = await _client.PublishConfigAsync(new PublishConfigRequest
{
    DataId = "dataId",
    Group = "DEFAULT_GROUP",
    //Tenant = "tenant",
    Content = "test"
});

// Delete configuration
var removeConfigResult = await _client.RemoveConfigAsync(new RemoveConfigRequest
{
    DataId = "dataId",
    Group = "DEFAULT_GROUP",
    //Tenant = "tenant"
});

```

### Service Discovery

```cs
// Register instance
var registerInstance = await _client.RegisterInstanceAsync(new RegisterInstanceRequest
{
    ServiceName = "testservice",
    Ip = "192.168.0.74",
    Port = 9999
});

// Deregister instance
var removeInstance = await _client.RemoveInstanceAsync(new RemoveInstanceRequest
{
    ServiceName = "testservice",
    Ip = "192.168.0.74",
    Port = 9999
});

// Modify instance
var modifyInstance = await _client.ModifyInstanceAsync(new ModifyInstanceRequest
{
    ServiceName = "testservice",
    Ip = "192.168.0.74",
    Port = 5000
});

// Query instances
var listInstances = await _client.ListInstancesAsync(new ListInstancesRequest
{
    ServiceName = "testservice",
});
   
// Query instance detail
var getInstance = await _client.GetInstanceAsync(new GetInstanceRequest
{
    ServiceName = "testservice",
    Ip = "192.168.0.74",
    Port = 9999,                 
});

// Send instance beat
var sendHeartbeat = await _client.SendHeartbeatAsync(new SendHeartbeatRequest
{
    ServiceName = "testservice",
    BeatInfo = new BeatInfo
    {
        ServiceName = "testservice",
        Ip = "192.168.0.74",
        Port = 9999,                     
    }
});
    
// Create service
var createService = await _client.CreateServiceAsync(new CreateServiceRequest
{
    ServiceName = "testservice"
});

// Delete service
var removeService = await _client.RemoveServiceAsync(new RemoveServiceRequest
{
    ServiceName = "testservice"
});

// Update service
var modifyService = await _client.ModifyServiceAsync(new ModifyServiceRequest
{
    ServiceName = "testservice",
    ProtectThreshold = 0.5,
});

// Query service
var getService = await _client.GetServiceAsync(new GetServiceRequest
{
    ServiceName = "testservice",
});

// Query service list
var listServices = await _client.ListServicesAsync(new ListServicesRequest
{
    PageNo = 1,
    PageSize = 2,
});

// Query system switches
var getSwitches = await _client.GetSwitchesAsync();

// Update system switch
var modifySwitches = await _client.ModifySwitchesAsync(new ModifySwitchesRequest
{
    Debug = true,
    Entry = "test",
    Value = "test"
});

// Query system metrics
var getMetricsres = await _client.GetMetricsAsync();

// Query server list
var listClusterServers = await _client.ListClusterServersAsync(new ListClusterServersRequest
{
        
});

// Query the leader of current cluster
var getCurrentClusterLeader = await _client.GetCurrentClusterLeaderAsync();

// Update instance health status
var modifyInstanceHealthStatus = await _client.ModifyInstanceHealthStatusAsync(new ModifyInstanceHealthStatusRequest
{
    Ip = "192.168.0.74",
    Port = 9999,
    ServiceName = "testservice",
    Healthy = false,
});
```


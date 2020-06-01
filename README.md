# nacos-sdk-csharp 　　　　　   　　   　　　[中文](./README.zh-cn.md)

Unofficial csharp(dotnet core) implementation of [nacos](https://nacos.io/) OpenAPI.

![](https://img.shields.io/nuget/v/nacos-sdk-csharp-unofficial.svg)

![](./media/prj.png)

## CI Build Status

| Platform | Build Server | Master Status  |
|--------- |------------- |---------|
| Github Action   | Linux/Windows |![nacos-sdk-csharp CI](https://github.com/catcherwong/nacos-sdk-csharp/workflows/nacos-sdk-csharp%20CI/badge.svg)

## Installation

```bash
dotnet add package nacos-sdk-csharp-unofficial
```

## Features

- Basic OpenApi Usages
- Integrate ASP.NET Core Configuration System
- Service Registration and Discovery With ASP.NET Core
- Integrate With Aliyun ACM
- ...

## Basic Usage

### Simple Configuration Usage

1. Configure in `Program.cs`

```cs
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((context, builder) =>
        {
            var c = builder.Build();

            // read configuration from config files
            builder.AddNacosConfiguration(c.GetSection("NacosConfig"));
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        })
```

2. Modify `appsettings.json`

```JSON
{
  "NacosConfig": {
    "Optional": false,
    "DataId": "msconfigapp",
    "Group": "",
    "Tenant": "f47e0ae1-982a-4a64-aea3-52506492a3d4",
    "ServerAddresses": [ "http://localhost:8848/" ],
    "UserName": "test2",
    "Password": "123456",
    "AccessKey": "",
    "SecretKey": "",
    "EndPoint": "acm.aliyun.com"
  }
}
```

3. Use via .NET Core's Way

```cs
[ApiController]
[Route("api/[controller]")]
public class ConfigController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly AppSettings _settings;
    private readonly AppSettings _sSettings;
    private readonly AppSettings _mSettings;
    
    public ConfigController(
        IConfiguration configuration,
        IOptions<AppSettings> options,
        IOptionsSnapshot<AppSettings> sOptions,
        IOptionsMonitor<AppSettings> _mOptions
        )
    {
        _logger = logger;
        _configuration = configuration;
        _settings = options.Value;
        _sSettings = sOptions.Value;
        _mSettings = _mOptions.CurrentValue;
    }

    [HttpGet]
    public string Get()
    {
        // ....
       
        return "ok";
    }

}
```

### Service Registration and Discovery

1. Service Registration

Configure in `Program.cs`

```cs
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // ...

        services.AddNacosAspNetCore(Configuration);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // ...
    }
}
```

Modify `appsettings.json`

```JSON
"nacos": {
    "ServerAddresses": [ "http://localhost:8848" ],
    "DefaultTimeOut": 15000,
    "Namespace": "",
    "ListenInterval": 1000,
    "ServiceName": "App1"
  }
```

2. Service Discovery

```cs
[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
    private readonly INacosServerManager _serverManager;

    public ValuesController(INacosServerManager serverManager)
    {
        _serverManager = serverManager;
    }

    [HttpGet("test")]
    public async Task<IActionResult> Test()
    {        
        // need to know the service name.
        // at this time only support random way.
        var baseUrl = await _serverManager.GetServerAsync("App2");
                    
        if(string.IsNullOrWhiteSpace(baseUrl))
        {
            return "empty";
        }

        var url = $"{baseUrl}/api/values";

        using (HttpClient client = new HttpClient())
        {
            var result = await client.GetAsync(url);
            return await result.Content.ReadAsStringAsync();
        }
    }
}
```

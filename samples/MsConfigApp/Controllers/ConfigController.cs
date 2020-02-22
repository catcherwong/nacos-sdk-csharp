using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MsConfigApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigController : ControllerBase
    {
        private readonly ILogger<ConfigController> _logger;
        private readonly IConfiguration _configuration;
        private readonly AppSettings _settings;
        private readonly AppSettings _sSettings;
        private readonly AppSettings _mSettings;
        
        public ConfigController(
            ILogger<ConfigController> logger, 
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
            string id = Guid.NewGuid().ToString("N");

            _logger.LogInformation($"============== begin {id} =====================");

            var conn = _configuration.GetConnectionString("Default");
            _logger.LogInformation($"{id} conn = {conn}");

            var version = _configuration["version"];
            _logger.LogInformation($"{id} version = {version}");

            var str1 = Newtonsoft.Json.JsonConvert.SerializeObject(_settings);
            _logger.LogInformation($"{id} IOptions = {str1}");

            var str2 = Newtonsoft.Json.JsonConvert.SerializeObject(_sSettings);
            _logger.LogInformation($"{id} IOptionsSnapshot = {str2}");

            var str3 = Newtonsoft.Json.JsonConvert.SerializeObject(_mSettings);
            _logger.LogInformation($"{id} IOptionsMonitor = {str3}");

            _logger.LogInformation($"===============================================");
            _logger.LogInformation($"===============================================");
            _logger.LogInformation($"===============================================");

            return "ok";
        }

    }

}

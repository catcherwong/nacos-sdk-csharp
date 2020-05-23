namespace Nacos.Config.Http
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    public abstract class HttpAgent : IHttpAgent
    {
        public async Task<HttpResponseMessage> DeleteAsync(string path, Dictionary<string, string> headers, Dictionary<string, string> paramValues, int timeout = 8000)
        {
            return await ReqApiAsync(HttpMethod.Delete, path, headers, paramValues, timeout);
        }

        public async Task<HttpResponseMessage> GetAsync(string path, Dictionary<string, string> headers, Dictionary<string, string> paramValues, int timeout = 8000)
        {
            return await ReqApiAsync(HttpMethod.Get, path, headers, paramValues, timeout);
        }

        public async Task<HttpResponseMessage> PostAsync(string path, Dictionary<string, string> headers, Dictionary<string, string> paramValues, int timeout = 8000)
        {
            return await ReqApiAsync(HttpMethod.Post, path, headers, paramValues, timeout);
        }

        public abstract Task<HttpResponseMessage> ReqApiAsync(HttpMethod httpMethod, string path, Dictionary<string, string> headers, Dictionary<string, string> paramValues, int timeout);
    }
}

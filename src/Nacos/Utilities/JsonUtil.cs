namespace Nacos.Utilities
{
    using Newtonsoft.Json;

    public static class JsonUtil
    {
        public static string ToJsonString(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T ToObj<T>(this string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return default(T);

            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}

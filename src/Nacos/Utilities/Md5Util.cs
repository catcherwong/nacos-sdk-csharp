namespace Nacos.Utilities
{
    using System.Security.Cryptography;
    using System.Text;

    public static class Md5Util
    {
        public static string GetMd5(string value)
        {
            var result = string.Empty;

            if (string.IsNullOrEmpty(value))
            {
                return result;
            }

            using (var md5 = MD5.Create())
            {
                byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(value));
                var sBuilder = new StringBuilder();
                foreach (byte t in data)
                {
                    sBuilder.Append(t.ToString("x2"));
                }
                result = sBuilder.ToString();
            }

            return result;
        }
    }
}

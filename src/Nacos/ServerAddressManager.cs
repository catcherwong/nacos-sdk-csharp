namespace Nacos
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ServerAddressManager
    {
        private string _server;

        private readonly List<string> _servers = new List<string>();

        private int _offset = 0;

        public ServerAddressManager(NacosOptions options)
        {
            var serverAddresses = options.ServerAddresses;

            if (serverAddresses == null || !serverAddresses.Any()) throw new ArgumentNullException(" ServerAddresses can not be null or empty ");

            foreach (var item in serverAddresses)
            {
                var hostAndPort = string.Empty;

                var tmp = item.Split(':');

                if (tmp.Length == 2)
                {
                    hostAndPort = item;
                }
                else if (tmp.Length == 1)
                {
                    hostAndPort = $"{tmp[0]}:8848";
                }
                else
                {
                    throw new ArgumentException(" incorrect server address, it should be [ip:port] ");
                }

                _servers.Add(hostAndPort);
            }

            if (_servers.Count <= 0) throw new Exceptions.NacosException("can not find out nacos server");

            _server = _servers[0];
        }

        public void ChangeServer()
        {
            _offset = (_offset + 1) % _servers.Count;
            _server = _servers[_offset];
        }

        public string GetCurrentServer()
        {
            return _server;
        }
    }
}

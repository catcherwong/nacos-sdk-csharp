namespace Nacos
{
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using System.IO;

    public class FileLocalConfigInfoProcessor : ILocalConfigInfoProcessor
    {
        private readonly string FAILOVER_BASE = Path.Combine(Directory.GetCurrentDirectory(), "nacos-data", "data");
        private readonly string SNAPSHOT_BASE = Path.Combine(Directory.GetCurrentDirectory(), "nacos-data", "snapshot");


        public async Task<string> GetFailoverAsync(string dataId, string group, string tenant)
        {
            string failoverFile;
            if (!string.IsNullOrEmpty(tenant))
            {
                failoverFile = Path.Combine(SNAPSHOT_BASE, "config-data-tenant", tenant, group);
            }
            else
            {
                failoverFile = Path.Combine(SNAPSHOT_BASE, "config-data", group);
            }
            var file = new FileInfo(failoverFile + dataId);

            if (!file.Exists)
            {
                return null;
            }

            var config = File.ReadAllText(file.FullName);

            return await Task.FromResult(config);
        }

        public async Task<string> GetSnapshotAync(string dataId, string group, string tenant)
        {
            FileInfo file = GetSnapshotFile(dataId, group, tenant);

            if (!file.Exists)
            {
                return null;
            }

            var config = File.ReadAllText(file.FullName);

            return await Task.FromResult(config);
        }

        private FileInfo GetSnapshotFile(string dataId, string group, string tenant)
        {
            string snapshotFile;
            if (!string.IsNullOrEmpty(tenant))
            {
                snapshotFile = Path.Combine(SNAPSHOT_BASE, "snapshot-tenant", tenant, group);
            }
            else
            {
                snapshotFile = Path.Combine(SNAPSHOT_BASE, "snapshot", group);
            }
            var file = new FileInfo(snapshotFile + dataId);
            return file;
        }

        public async Task SaveSnapshotAsync(string dataId, string group, string tenant, string config)
        {
            FileInfo snapshotFile = GetSnapshotFile(dataId, group, tenant);
            if (string.IsNullOrEmpty(config))
            {
                if (snapshotFile.Exists)
                {
                    snapshotFile.Delete();
                }
            }
            else
            {
                if (snapshotFile.Directory != null && !snapshotFile.Directory.Exists)
                {
                    snapshotFile.Directory.Create();
                }

                File.WriteAllText(snapshotFile.FullName, config);
            }

            await Task.Yield();
        }
    }
}

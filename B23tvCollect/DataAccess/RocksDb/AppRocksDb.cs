using RocksDbSharp;
using System.Data.Common;

namespace B23tvCollect.DataAccess.RocksDb
{
    public class AppRocksDb
    {
        public RocksDbSharp.RocksDb Db { get; }

        public AppRocksDb(RocksDbSharp.DbOptions dbOptions, string dbPath)
        {
            List<string> existCfNames = new List<string>();
            if (RocksDbSharp.RocksDb.TryListColumnFamilies(new DbOptions(), dbPath, out string[] cf))
            {
                existCfNames.AddRange(cf);
            }
            if (existCfNames.Count == 0)
            {
                existCfNames.Add("default");
            }
            var cfs = new ColumnFamilies();
            var cfOptions = new ColumnFamilyOptions();
            cfOptions.SetCompression(Compression.Snappy);
            cfOptions.SetCreateIfMissing(true);

            foreach (var currentCf in existCfNames)
            {

                cfs.Add(currentCf, cfOptions);
            }

            Db = RocksDbSharp.RocksDb.Open(dbOptions, dbPath, cfs);

            //默认列族
            AddIfNotExist("LastRecordId", cfOptions, existCfNames, Db);
            AddIfNotExist("Target", cfOptions, existCfNames, Db);
            AddIfNotExist("TargetType", cfOptions, existCfNames, Db);
            AddIfNotExist("SubmitTime", cfOptions, existCfNames, Db);
            //AddIfNotExist("InnerCode", cfOptions, existCfNames, Db);


            void AddIfNotExist(string name, ColumnFamilyOptions options, List<string> list, RocksDbSharp.RocksDb db)
            {
                if (!list.Contains(name)) db.CreateColumnFamily(options, name);
            }
        }
    }
}

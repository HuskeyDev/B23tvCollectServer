using B23tvCollect.Common;
using B23tvCollect.Models;
using Microsoft.VisualBasic;
using RocksDbSharp;
using System.Text;
using System;
namespace B23tvCollect.DataAccess.RocksDb.Operations
{
    public class CollectOperation
    {
        private readonly RocksDbSharp.RocksDb _rocksDb;
        private static readonly Encoding _utf8 = Encoding.UTF8;
        private static readonly char Delimiter = '_';
        private ColumnFamilyHandle LastRecordIdColumn;
        private ColumnFamilyHandle TargetColumn;
        private ColumnFamilyHandle TargetTypeColumn;
        private ColumnFamilyHandle SubmitTimeColumn;
        private static readonly object RecordIdIncrementLock = new object();
        public CollectOperation(RocksDbSharp.RocksDb db)
        {
            _rocksDb = db;
            LastRecordIdColumn = _rocksDb.GetColumnFamily("LastRecordId");
            TargetColumn = _rocksDb.GetColumnFamily("Target");
            TargetTypeColumn = _rocksDb.GetColumnFamily("TargetType");
            SubmitTimeColumn = _rocksDb.GetColumnFamily("SubmitTime");
        }
        public string GetKeyName(string b23tvCode, string id)
        {
            return b23tvCode + Delimiter + id;
        }
        public int GetLastRecordId(string b23tvCode)
        {
            int lastRecordId = -1;
            var lastRecordIdCache = _rocksDb.Get(b23tvCode, LastRecordIdColumn);
            if (!string.IsNullOrEmpty(lastRecordIdCache))
            {
                lastRecordId = Convert.ToInt32(lastRecordIdCache);
            }
            return lastRecordId;
        }
        public int NewRecordId(string b23tvCode)
        {
            lock (RecordIdIncrementLock)
            {
                var lastRecordIdCache = _rocksDb.Get(b23tvCode, LastRecordIdColumn);
                if (string.IsNullOrEmpty(lastRecordIdCache)) lastRecordIdCache = "-1";
                var recordId = int.Parse(lastRecordIdCache) + 1;
                _rocksDb.Put(b23tvCode, recordId.ToString(), LastRecordIdColumn);
                return recordId;
            }
        }
        public bool CheckIsRecordExisted(string b23tvCode,string target)
        {
            var seekKey = GetKeyName(b23tvCode, string.Empty);//分隔符
            var start = _utf8.GetBytes(seekKey);
            var end = (byte[])start.Clone();
            end[end.Length - 1]++;
            ReadOptions readOptions = new ReadOptions();
            readOptions.SetIterateUpperBound(end);
            using Iterator iterator = _rocksDb.NewIterator(TargetColumn, readOptions);
            iterator.Seek(start);
            while (iterator.Valid())
            {
                if (_utf8.GetString(iterator.Value()) == target) return true;
                iterator.Next();
            }
            return false;
        }
        public void WriteNewRecord(string b23tvCode, string target, int targetType, long submitTime)
        {
            var id = (NewRecordId(b23tvCode)).ToString();
            var key = GetKeyName(b23tvCode, id);
            //throw new Common.Exceptions.BusinessException(114514,"1");//业务错误测试，生产删
            using WriteBatch writeBatch = new WriteBatch();
            writeBatch.Put(_utf8.GetBytes(key), _utf8.GetBytes(target), TargetColumn);
            writeBatch.Put(_utf8.GetBytes(key), _utf8.GetBytes(targetType.ToString()), TargetTypeColumn);
            writeBatch.Put(_utf8.GetBytes(key), _utf8.GetBytes(submitTime.ToString()), SubmitTimeColumn);
            _rocksDb.Write(writeBatch);
        }
        public List<Models.Common.InnerTargetObject> FindAllRecordsByB23tvCode(string b23tvCode)
        {
            var targetObjects = new List<Models.Common.InnerTargetObject>();
            //var columnFamilyList = new List<ColumnFamilyHandle>();
            var keyList = new List<byte[]>();
            //标准key XXXXX_num
            var seekKey = GetKeyName(b23tvCode, string.Empty);//分隔符
            var start = _utf8.GetBytes(seekKey);
            var end = (byte[])start.Clone();
            end[end.Length - 1]++;
            ReadOptions readOptions = new ReadOptions();
            readOptions.SetIterateUpperBound(end);
            using Iterator iterator = _rocksDb.NewIterator(TargetColumn, readOptions);
            iterator.Seek(start);
            while (iterator.Valid())
            {
                keyList.Add(iterator.Key());
                iterator.Next();
            }
            if (keyList.Count == 0) { return targetObjects; }//未找到，免去下面操作

            var targetKVPairs = _rocksDb.MultiGet(keyList.ToArray(), Enumerable.Repeat(TargetColumn, keyList.Count).ToArray());
            var targetTypeKVPairs = _rocksDb.MultiGet(keyList.ToArray(), Enumerable.Repeat(TargetTypeColumn, keyList.Count).ToArray());
            var submitTimeKVPairs = _rocksDb.MultiGet(keyList.ToArray(), Enumerable.Repeat(SubmitTimeColumn, keyList.Count).ToArray());
            //var innerCodeKVPairs = _rocksDb.MultiGet(keyList.ToArray(), Enumerable.Repeat(InnerCodeColumn, keyList.Count).ToArray());

            for (var i = 0; i < keyList.Count; i++)
            {
                var target = _utf8.GetString(targetKVPairs[i].Value);
                var targetType = int.Parse(targetTypeKVPairs[i].Value, System.Globalization.CultureInfo.InvariantCulture);
                var submitTime = long.Parse(submitTimeKVPairs[i].Value, System.Globalization.CultureInfo.InvariantCulture);
                var key = _utf8.GetString(keyList[i]);
                var targetId = key.Substring(key.LastIndexOf(Delimiter) + 1);
                var currentTargetObject = new Models.Common.InnerTargetObject();
                currentTargetObject.target = target;
                currentTargetObject.targetType = targetType;
                currentTargetObject.submitTime = submitTime;
                currentTargetObject.targetId = Convert.ToInt32(targetId);
                targetObjects.Add(currentTargetObject);
            }
            return targetObjects;
        }
    }
}

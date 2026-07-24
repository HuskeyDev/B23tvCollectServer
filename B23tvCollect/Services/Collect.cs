using B23tvCollect.Common;
using B23tvCollect.Common.Exceptions;
using B23tvCollect.Common.Helpers;
using B23tvCollect.DataAccess.RocksDb;
using B23tvCollect.DataAccess.RocksDb.Operations;
using B23tvCollect.Models;
using static B23tvCollect.Models.Common;
namespace B23tvCollect.Services
{
    public class Collect
    {
        public readonly CollectOperation _collect;
        public Collect(AppRocksDb rocksDb)
        {
            _collect = new CollectOperation(rocksDb.Db);
        }
        public void NewRecord(string b23tvCode, string target)
        {
            if (string.IsNullOrEmpty(b23tvCode)) throw new BusinessException(600, "参数为空：b23tvCode");
            if (string.IsNullOrEmpty(target)) throw new BusinessException(601, "参数为空：target");
            if (UriHelper.IsValidUrl(b23tvCode)) b23tvCode = UriHelper.GetUri(b23tvCode).Segments.Last().TrimEnd('/');
            var targetType = TargetTypeHelper.GetTargetType(target);
            target = TargetTypeHelper.FormatStorageContent(targetType, target);
            if (_collect.CheckIsRecordExisted(b23tvCode, target)) throw new BusinessException(603, "已存在相同记录");
            _collect.WriteNewRecord(b23tvCode, target, targetType, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        }
        public Models.LinkResponse.ReturnTarget FindAllRecordsByB23(string b23tvCode)
        {
            if (string.IsNullOrEmpty(b23tvCode)) throw new BusinessException(600, "参数为空：b23tvCode");
            if(UriHelper.IsValidUrl(b23tvCode)) b23tvCode = UriHelper.GetUri(b23tvCode).Segments.Last().TrimEnd('/');
            var records = _collect.FindAllRecordsByB23tvCode(b23tvCode);
            if (records.Count == 0) throw new BusinessException(604, "无匹配");
            var returnObject = new LinkResponse.ReturnTarget();
            foreach (var record in records)
            {
                var returnTarget = new TargetObject() {
                    targetId = record.targetId,
                    target = TargetTypeHelper.GetTargetUrl(record.targetType, record.target),
                    targetType = record.targetType,
                    submitTime = record.submitTime,
                };
                returnObject.targets.Add(returnTarget);
            }
            return returnObject;
        }
    }
}

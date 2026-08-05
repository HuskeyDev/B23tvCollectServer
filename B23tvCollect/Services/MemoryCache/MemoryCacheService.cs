using B23tvCollect.DataAccess.RocksDb;
using B23tvCollect.Services.MemoryCache;
using Serilog;

namespace B23tvCollect.Services.MemoryCache
{
    public class MemoryCacheService : BackgroundService
    {
        private readonly AppRocksDb _rocksDb;
        private readonly TimeSpan FlushInterval = TimeSpan.FromMinutes(5);
        public MemoryCacheService(AppRocksDb rocksDb)
        {
            _rocksDb = rocksDb;
        }
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            Log.Information("内存缓存服务启动");
            LoadFromDb();
            await base.StartAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Log.Information("内存缓存定时固化启动");
                FlushToDb();
                await Task.Delay(FlushInterval, stoppingToken);
            }
        }
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            Log.Information("内存缓存服务停止");
            FlushToDb();
            Log.Information("内存缓存服务停止成功");
            await base.StopAsync(cancellationToken);
        }
        private void LoadFromDb()
        {
            Log.Information("从数据库加载缓存中");

            Log.Information("缓存加载成功");
        }
        private void FlushToDb()
        {
            Log.Information("固化缓存到数据库中");

            Log.Information("缓存固化成功");
        }
    }
}
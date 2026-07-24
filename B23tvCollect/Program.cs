
using B23tvCollect.Common.Constants;
using B23tvCollect.Common.Exceptions;
using B23tvCollect.DataAccess.RocksDb;
using Microsoft.AspNetCore.Mvc;
using RocksDbSharp;
using Serilog;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace B23tvCollect
{
    public class Program
    {
        public static void Main(string[] args)
        {
#if DEBUG
            var defaultDebugDir = System.Environment.CurrentDirectory;
            //调试模式下，暂时设置工作目录为可执行文件目录
            System.Environment.CurrentDirectory = DirConst.ExeDir;
#endif
            //配置文件
            JsonObject config = new JsonObject();
            config["logDir"] = DirConst.LogDir;
            config["dataBaseDir"] = DirConst.DataBaseDir;
            config["apiRoute"] = "api" + '/' + Assembly.GetExecutingAssembly().GetName().Name;

            Directory.CreateDirectory(DirConst.ConfigDir);
            var configFilePath = Path.Combine(DirConst.ConfigDir, "Config.json");
            if (!File.Exists(configFilePath))
            {
                using (StreamWriter sw = File.CreateText(configFilePath))
                {
                    sw.Write(config.ToString());
                }
            }
            string configFileText = File.ReadAllText(configFilePath);
            config = JsonNode.Parse(configFileText) as JsonObject;

            DirConst.LogDir = Path.GetFullPath(config["logDir"].ToString());
            DirConst.DataBaseDir = Path.GetFullPath(config["dataBaseDir"].ToString());
#if DEBUG
            System.Environment.CurrentDirectory = defaultDebugDir;
#endif

            //日志
            var loggerConfiguration = new LoggerConfiguration();
            loggerConfiguration.MinimumLevel.Debug();
            loggerConfiguration.WriteTo.File(
                Path.Combine(DirConst.LogDir, ".log"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                shared: true);
            Log.Logger = loggerConfiguration.CreateLogger();

            //数据库
            var dbOptions = new DbOptions().SetCreateIfMissing(true);
            string dbPath = Path.Combine(AppContext.BaseDirectory, "DataBase");

            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.

            //添加异常处理器
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

            //添加数据库单例
            builder.Services.AddSingleton(new AppRocksDb(dbOptions, dbPath));

            builder.Services.AddControllers();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            //全局异常处理
            app.UseExceptionHandler(_ => { });

            app.UseHttpsRedirection();

            app.UseAuthorization();

            //app.MapControllers();
            var apiGroup = app.MapGroup(config["apiRoute"].ToString());
            apiGroup.MapControllers();

            Log.Information("初始化成功，程序启动");

            app.Run();
        }
    }
}

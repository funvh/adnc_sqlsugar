using Adnc.Infra.Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SerilogServiceConllectionExtension
    {
        /// <summary>
        /// 注入Serilog服务
        /// </summary>
        /// <param name="service"></param>
        /// <param name="configration"></param>
        /// <param name="loggerName"></param>
        /// <returns></returns>
        public static IServiceCollection AddSerilogService(this IServiceCollection service, IConfiguration configration, string loggerName)
        {
            //替换日志框架，换成 Serilog
            service.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();

                var serilogConfig = configration.GetSection(SerilogConfigOptions.OptionName).Get<SerilogConfigOptions>();

                var loggerConfig = new LoggerConfiguration()
                    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
                    .MinimumLevel.Information()
                    .WriteTo.Console(
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff}|[{Level:u3}]|{Message:lj}{Exception}|{SourceContext}|{ThreadId}{NewLine}"
                        )
                    .WriteTo.File(
                        path: "Logs/log-.txt",
                        //formatter: "Serilog.Formatting.Compact.CompactJsonFormatter",
                        rollingInterval: RollingInterval.Day,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff}|[{Level:u3}]|{Message:lj}{Exception}|{SourceContext}|{ThreadId}{NewLine}{NewLine}",
                        shared: true,
                        rollOnFileSizeLimit: true,
                        fileSizeLimitBytes: 102400000,
                        retainedFileCountLimit: 365);

                if (serilogConfig != null)
                {
                    if (serilogConfig.MongoDBUrl.IsNotNullOrWhiteSpace())
                    {
                        loggerConfig = loggerConfig.WriteTo.MongoDBBson(serilogConfig.MongoDBUrl,
                            collectionName: loggerName,
                            cappedMaxDocuments: 50000);
                    }

                    if (serilogConfig.SeqUrl.IsNotNullOrWhiteSpace())
                    {
                        loggerConfig = loggerConfig.WriteTo.Seq(serilogConfig.SeqUrl, queueSizeLimit: 10000);
                    }
                }

                //配置Log
                Log.Logger = loggerConfig.CreateLogger();

                loggingBuilder.AddSerilog(Log.Logger);
            });

            return service;
        }
    }
}

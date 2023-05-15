using System.Data;
using System.Data.SqlClient;
using LayherDelPacifico.Core.DTO;
using LayherDelPacifico.Core.DTO.Documents;
using LayherDelPacifico.Core.Interfaces;
using LayherDelPacifico.Core.Services;
using LayherDelPacifico.Infrastructure.Repository;
using LayherDelPacifico.Worker;
using NLog.Extensions.Logging;
using System.Linq;
using System.Net;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var dbConfig = context.Configuration.GetSection("ConnectionStrings").Get<ConnectionStrings>();
        var pathConfig = context.Configuration.GetSection("PathsConfiguration").Get<PathsConfiguration>();
        var documentList = context.Configuration.GetSection("TypesDocument").Get<TypesDocument>();
        var ftpConfig = context.Configuration.GetSection("FtpConfiguration").Get<FtpConfiguration>();

        services.AddSingleton<IAgiliceDataBase, AgiliceDataBase>();
        services.AddSingleton<IWatcherFolder, WatcherFolder>();
        services.AddSingleton<IDbConnection>((sp) => new SqlConnection(dbConfig.AgiliceConnectionString));
        services.AddSingleton<IDictionary<string, string>>((dictionary) => documentList.Documents.ToDictionary(x => x.Name, x => x.Type));
        services.AddSingleton<ILayFtp, LayFtp>();

        services.AddHostedService<Worker>();

    }).UseWindowsService(options =>
    {
        options.ServiceName = "Change name folder Asia Shipping";
    }).ConfigureLogging((context, log) =>
    {
        var logLevelConfig = context.Configuration.GetSection("NLogConfiguration").Get<NLogConfiguration>();
        LogLevel logLevel = (LogLevel)Enum.Parse(typeof(LogLevel), logLevelConfig.NLogLevel, true);
        log.ClearProviders();
        log.SetMinimumLevel(logLevel);
        log.AddNLog(context.Configuration);

    }).Build();

await host.RunAsync();
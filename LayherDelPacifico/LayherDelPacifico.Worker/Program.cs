using System.Data;
using System.Data.SqlClient;
using LayherDelPacifico.Core.DTO;
using LayherDelPacifico.Core.Interfaces;
using LayherDelPacifico.Core.Services;
using LayherDelPacifico.Infrastructure.Repository;
using LayherDelPacifico.Worker;
using NLog.Extensions.Logging;


IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var dbConfig = context.Configuration.GetSection("ConnectionStrings").Get<ConnectionStrings>();
        var pathConfig = context.Configuration.GetSection("PathsConfiguration").Get<PathsConfiguration>();

        services.AddSingleton<IAgiliceDataBase, AgiliceDataBase>();
        services.AddSingleton<IWatcherFolder, WatcherFolder>();

        services.AddSingleton<IDbConnection>((sp) => new SqlConnection(dbConfig.AgiliceConnectionString));
        services.AddHostedService<Worker>();

    }).UseWindowsService(options =>
    {
        options.ServiceName = "Change name folder Asia Shipping";
    }).ConfigureLogging((context, log) =>
    {
        log.ClearProviders();
        log.SetMinimumLevel(LogLevel.Error);
        log.AddNLog(context.Configuration);

    }).Build();

await host.RunAsync();
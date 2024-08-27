using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using LayherDelPacifico.Core.Interfaces;
using LayherDelPacifico.Core.Services;
using LayherDelPacifico.Core.DTO;

namespace LayherDelPacifico.Worker
{
    public class Worker : BackgroundService
    {
        private readonly IWatcherFolder _watcherFolder;
        private readonly IConfiguration _configuration;
        private readonly IPurgeLog _purgeLog;
        private readonly ILogger<Worker> _logger;
        public Worker(ILogger<Worker> logger, IConfiguration configuration, IWatcherFolder watcherFolder, IPurgeLog purgeLog)
        {
            _configuration = configuration;
            _watcherFolder = watcherFolder;
            _purgeLog = purgeLog;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {                    
                    var pathConfig = _configuration.GetSection("PathsConfiguration").Get<PathsConfiguration>();
                    var ftpConfig = _configuration.GetSection("FtpConfiguration").Get<FtpConfiguration>();
                    var rutsEmisores = _configuration.GetSection("RutsEmisores").Get<RutsEmisores>();
                    var logConfig = _configuration.GetSection("NLogConfiguration").Get<NLogConfiguration>();
                    await _watcherFolder.Watcher(pathConfig, ftpConfig, _logger,rutsEmisores.Ruts);
                    await _purgeLog.Purge(logConfig.LogPath, logConfig.MaxFiles);
                    await Task.Delay(Timeout.Infinite, stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    _logger.LogInformation(ex.Message, ex);
                }
            }
        }
    }
}
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
        private readonly ILogger<Worker> _logger;
        public Worker(ILogger<Worker> logger, IConfiguration configuration, IWatcherFolder watcherFolder)
        {
            _configuration = configuration;
            _watcherFolder = watcherFolder;
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
                    await _watcherFolder.Watcher(pathConfig, ftpConfig, _logger);
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
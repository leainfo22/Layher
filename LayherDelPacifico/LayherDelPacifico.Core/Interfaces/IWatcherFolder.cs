using LayherDelPacifico.Core.DTO;
using Microsoft.Extensions.Logging;


namespace LayherDelPacifico.Core.Interfaces
{
    public interface IWatcherFolder
    {
        public Task Watcher(PathsConfiguration pathConfig, FtpConfiguration ftpConfig, ILogger logger);
    }
}

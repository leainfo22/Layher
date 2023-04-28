using Microsoft.Extensions.Logging;


namespace LayherDelPacifico.Core.Interfaces
{
    public interface IWatcherFolder
    {
        public Task Watcher(string pathIn, string pathOut, ILogger logger);
    }
}

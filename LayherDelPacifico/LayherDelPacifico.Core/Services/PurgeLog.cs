using LayherDelPacifico.Core.DTO;
using LayherDelPacifico.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace LayherDelPacifico.Core.Services
{
    public class PurgeLog : IPurgeLog
    {
        private static System.Timers.Timer aTimer;
        private static int _maxFiles;
        private static string _logPath;

        public async Task Purge(string logPath, int maxFiles) 
        {
            _maxFiles = maxFiles;
            _logPath = logPath;
            aTimer = new System.Timers.Timer();
            aTimer.Enabled = true;
            aTimer.Interval = 86400 * 1000;
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            GC.KeepAlive(aTimer);
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            var logsFiles = new DirectoryInfo(_logPath).GetFiles().OrderBy(f => f.LastWriteTime).ToList();
            if (logsFiles.Count > _maxFiles) 
            {
                try
                {
                    for(int i = 0; i<logsFiles.Count - _maxFiles;i++)
                        File.Delete(logsFiles[i].FullName);                    
                }
                catch
                {
                    return;
                }
            }
        }

    }
}

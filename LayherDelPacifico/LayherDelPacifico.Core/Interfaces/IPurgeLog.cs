using LayherDelPacifico.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayherDelPacifico.Core.Interfaces
{
    public interface IPurgeLog
    {
        public Task Purge(string logPath, int maxFiles);
    }
}

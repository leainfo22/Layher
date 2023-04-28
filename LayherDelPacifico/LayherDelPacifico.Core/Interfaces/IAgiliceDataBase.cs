using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayherDelPacifico.Core.Interfaces
{
    public interface IAgiliceDataBase
    {
        public Task<string> GetXml(string folio, string tipoDoc, string rutEmisor);
    }
}

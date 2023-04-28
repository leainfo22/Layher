using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayherDelPacifico.Infrastructure.Repository
{
    public class TypeDocDataFile
    {
        private readonly IDictionary<string, string> _tipoDeDoc;

        
        public TypeDocDataFile(IDictionary<string,string> tipoDeDoc) 
        {
            _tipoDeDoc = tipoDeDoc;

        }

    }
}

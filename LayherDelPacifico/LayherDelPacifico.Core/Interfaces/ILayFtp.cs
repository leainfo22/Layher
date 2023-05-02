using LayherDelPacifico.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayherDelPacifico.Core.Interfaces
{
    public interface ILayFtp
    {
        public Task UploadFile(FtpConfiguration ftpConfig, string localFilePath, string fileName);  

    }
}

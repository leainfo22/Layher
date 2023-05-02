using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayherDelPacifico.Core.DTO
{
    public class FtpConfiguration
    {
        public string FtpServerPath { get; set; }
        public string FtpUser { get; set; }
        public string FtpPassword { get; set; }
        public string? FtpPort { get; set; }

    }
}

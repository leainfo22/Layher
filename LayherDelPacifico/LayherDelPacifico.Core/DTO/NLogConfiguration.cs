﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayherDelPacifico.Core.DTO
{
    public class NLogConfiguration
    {
        public string LogPath { get; set; }
        public string NLogLevel { get; set; }
        public int MaxFiles { get; set; }

    }
}

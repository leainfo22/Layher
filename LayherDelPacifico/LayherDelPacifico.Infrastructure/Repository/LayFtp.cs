using LayherDelPacifico.Core.DTO;
using LayherDelPacifico.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LayherDelPacifico.Infrastructure.Repository
{
    public class LayFtp : ILayFtp
    {
        public async Task UploadFile(FtpConfiguration ftpConfig, string localFilePath, string fileName) 
        {
            try 
            {
                var _client = (FtpWebRequest)WebRequest.Create(new Uri(ftpConfig.FtpServerPath + fileName));
                _client.Method = WebRequestMethods.Ftp.UploadFile;
                _client.Credentials = new NetworkCredential(ftpConfig.FtpUser, ftpConfig.FtpPassword);

                using (Stream fileStream = File.OpenRead(localFilePath))
                {
                    using (Stream ftpStream = _client.GetRequestStream())
                    {
                        await fileStream.CopyToAsync(ftpStream);
                    }
                }
            } 
            catch (Exception ex)
            {
            
            }
            
        }
    }
}

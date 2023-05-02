using LayherDelPacifico.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using LayherDelPacifico.Core.DTO;

namespace LayherDelPacifico.Core.Services
{
    public class WatcherFolder : IWatcherFolder
    {
        private static string? _pathIn;
        private static string? _pathOut;
        private static ILogger? _logger;
        private readonly IAgiliceDataBase _agiliceDataBaseRepository;
        private readonly ILayFtp _layFtp;
        private readonly IDictionary<string,string> _tipoDoc;
        private  FtpConfiguration _ftpConfig;
        private static FileSystemWatcher watcher;


        public WatcherFolder(IAgiliceDataBase agiliceDataBaseRepository, IDictionary<string, string> tipoDoc,ILayFtp layFtp)
        {
            _agiliceDataBaseRepository = agiliceDataBaseRepository;
            _tipoDoc = tipoDoc;
            _layFtp = layFtp;
        }
        public async Task Watcher(PathsConfiguration pathConfig, FtpConfiguration ftpConfig, ILogger logger)
        {
            _ftpConfig = ftpConfig;
            _pathIn = pathConfig.PathIn;
            _pathOut = pathConfig.PathOut;
            _logger = logger;
            watcher = new FileSystemWatcher() { Path = _pathIn };
            watcher.IncludeSubdirectories = true;
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Error += new ErrorEventHandler(WatcherError);
            watcher.EnableRaisingEvents = true;
            GC.KeepAlive(watcher);

        }
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            _logger.LogInformation("Evento capturado");
            if (e.FullPath.Contains(".pdf") && !e.FullPath.Contains("-Cedible"))
            {
                _logger.LogInformation("PDF Capturado");
                CreateFileToMove(e.FullPath);
            }
        }

        private void CreateFileToMove(string path)
        {
            var values = path.Substring(path.IndexOf(_pathIn) + _pathIn.Length);
            _logger.LogInformation("Substirng " + values);
            var splitValues = values.Split('\\');

            var rutEmisor = splitValues[0];
            _logger.LogInformation("rutEmisor " + rutEmisor);

            var tipoDocNumber = _tipoDoc[splitValues[1]];
            _logger.LogInformation("tipoDocNumber " + tipoDocNumber);

            var splitFolio = splitValues[splitValues.Length - 1].Split('.');
            var folio = splitFolio[0];
            _logger.LogInformation("folio " + folio);

            var xml = _agiliceDataBaseRepository.GetXml(folio, tipoDocNumber, rutEmisor).Result;
            _logger.LogInformation("xml capturado");
            if (xml.Contains("ExceptionError"))
                return;
            var cutXml = ExtactXml(xml, "<TpoDocRef>I04</TpoDocRef>", "</RazonRef>");
            int pFrom = cutXml.IndexOf("<RazonRef>") + "<RazonRef>".Length;
            int pTo = cutXml.IndexOf("</RazonRef>");
            var razonRef = cutXml.Substring(pFrom, pTo - pFrom);
            _logger.LogInformation("razonRef " + razonRef);
            var outputFileName = string.Format("{0}_{1}_{2}.pdf", tipoDocNumber, folio, razonRef);

            if (!File.Exists(Path.Combine(_pathOut, outputFileName)))
            {
                TryToCopyFile(path, Path.Combine(_pathOut, outputFileName));
                _layFtp.UploadFile(_ftpConfig, path, outputFileName);
            }
        }

        private static string ExtactXml(string str, string strFrom, string strTo)
        {
            int pFrom = str.IndexOf(strFrom) + strFrom.Length;
            int pTo = str.LastIndexOf(strTo);
            return str.Substring(pFrom, pTo - pFrom);
        }
        private static void TryToCopyFile(string pathFrom, string pathTo)
        {
            const int NumberOfRetries = 3;
            const int DelayOnRetry = 3000;

            for (int i = 1; i <= NumberOfRetries; ++i)
            {
                try
                {
                    FileInfo fi = new FileInfo(pathFrom);
                    fi.CopyTo(pathTo, true);
                    break; // When done we can break loop
                }
                catch (IOException e) when (i <= NumberOfRetries)
                {
                    _logger.LogError("Intento " + i + " fallido");
                    Thread.Sleep(DelayOnRetry);
                }
            }
        }

        private void WatcherError(object source, ErrorEventArgs e)
        {
            _logger.LogError("WatcherError: " + e.ToString());
        }
    }
}

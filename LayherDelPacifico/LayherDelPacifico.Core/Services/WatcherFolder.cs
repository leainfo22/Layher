using LayherDelPacifico.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using LayherDelPacifico.Core.DTO;
using System.Timers;

namespace LayherDelPacifico.Core.Services
{
    public class WatcherFolder : IWatcherFolder
    {
        private static string? _pathIn;
        private static string? _pathOut;
        private static List<string> _rutsEmisores;
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
        public async Task Watcher(PathsConfiguration pathConfig, FtpConfiguration ftpConfig, ILogger logger, string rutsEmisores)
        {
            _ftpConfig = ftpConfig;
            _pathIn = pathConfig.PathIn;
            _pathOut = pathConfig.PathOut;
            _rutsEmisores = rutsEmisores.Split('|').ToList();
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
            if (e.FullPath.Contains(".pdf") && _rutsEmisores.Any(s=>e.FullPath.Contains(s)) && !e.FullPath.Contains("-Cedible"))
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
            var xml = _agiliceDataBaseRepository.GetXml(folio, tipoDocNumber, rutEmisor).Result;
            //se completa de ceros a la izquiera filio hasta completar 10 unidades

            var length = folio.Length;
            for (int i = 0; i < (10 - length); i++)
                folio = "0" + folio;
            _logger.LogInformation("folio " + folio);

            _logger.LogInformation("xml capturado");
            if (xml.Contains("ExceptionError"))
                return;
            int pFrom = xml.IndexOf("<CdgIntRecep>") + "<CdgIntRecep>".Length;
            int pTo = xml.IndexOf("</CdgIntRecep>");
            var CdgIntRecep = xml.Substring(pFrom, pTo - pFrom);
            length = CdgIntRecep.Length;
            for (int i = 0; i < (10 - length); i++)
                CdgIntRecep = CdgIntRecep + "%";
            _logger.LogInformation("CdgIntRecep " + CdgIntRecep);

            pFrom = xml.IndexOf("<RUTRecep>") + "<RUTRecep>".Length;
            pTo = xml.IndexOf("</RUTRecep>");
            var RutRecep = xml.Substring(pFrom, pTo - pFrom);
            RutRecep = RutRecep.Split('-')[0];
            _logger.LogInformation("RUTRecep " + RutRecep);

            var outputFileName = string.Format("{0}-{1}-{2}-{3}.pdf",tipoDocNumber, folio, RutRecep, CdgIntRecep);

            if (!File.Exists(Path.Combine(_pathOut, outputFileName)))
            {
                TryToCopyFile(path, Path.Combine(_pathOut, outputFileName));
                _layFtp.UploadFile(_ftpConfig, path, outputFileName);
            }
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

using Dapper;
using System.Data;
using LayherDelPacifico.Core.DTO.DB;
using LayherDelPacifico.Core.Interfaces;


namespace LayherDelPacifico.Infrastructure.Repository
{
	public class AgiliceDataBase : IAgiliceDataBase
	{

		private readonly IDbConnection _connection;

		public AgiliceDataBase(IDbConnection connection)
		{
			_connection = connection;
		}
		public async Task<string> GetXml(string folio, string tipoDoc,string rutEmisor)
		{
			try
			{
				var idEmpresa = await GetIdEmpresa(rutEmisor);
				if(idEmpresa.Contains("ExceptionError"))
					return "ExceptionError";

				var idDocumento = await GetIdDocumento(folio,tipoDoc,idEmpresa);
				if (idDocumento.Contains("ExceptionError"))
					return "ExceptionError";

				var sql = String.Format(@"SELECT Xml FROM Dte.XmlDte WHERE DocumentoId={0} ", idDocumento);
				var xml = _connection.QueryAsync<XmlDoc>(sql);

				return xml.Result.First().Xml;
			}
			catch (Exception ex)
			{
				return "ExceptionError" + ex.Message;
			}

		}
		public async Task<string> GetIdEmpresa(string rutEmisor)
		{
			try
			{
				var sql = String.Format(@"SELECT Id FROM Hub.Empresas WHERE Rut={0}",rutEmisor);
				var idEmpresa = await _connection.QueryAsync<IdEmpresa>(sql);
				return idEmpresa.First().Id;
			}
			catch (Exception ex)
			{
				return "ExceptionError" + ex.Message;
			}

		}
		public async Task<string> GetIdDocumento(string folio, string tipoDoc, string idEmpresa)
		{
			try
			{
				var sql = String.Format(@"SELECT Id FROM Dte.Documentos WHERE EmisorId={0} AND TipoDocumento={1} AND Folio={2}", idEmpresa, tipoDoc, folio);
				var idDocumento =await _connection.QueryAsync<IdDocumento>(sql);
				return idDocumento.First().Id;
			}
			catch (Exception ex)
			{
				return "ExceptionError" + ex.Message;
			}

		}
	}
}

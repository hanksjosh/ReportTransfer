
using System;
using System.Net;
using ReportTransfer.ReportService;

namespace ReportTransfer
{
	public class RSCommunicator : IRSCommunicator
	{
		private ReportingService2005 _reportService;

		public RSCommunicator(string url, string userName = null, string password = null)
		{
			_reportService = new ReportingService2005();
			_reportService.Url = url;
			_reportService.PreAuthenticate = true;
			if (String.IsNullOrWhiteSpace(userName) || String.IsNullOrWhiteSpace(password))
			{
				_reportService.UseDefaultCredentials = true;
			}
			else
			{
				_reportService.Credentials = new NetworkCredential(userName, password);
			}
		}

		public CatalogItem[] GetExistingReports(string folder)
		{
			string reportPath = "/" + folder;
			var results = _reportService.ListChildren(reportPath, true);
			return results;
		}

		public void DeleteAllReports(string folder)
		{
			string path = "/" + folder;
			_reportService.DeleteItem(path);
		}

		public void CreateFolder(string folderName, string parentPath)
		{
			try
			{
				_reportService.CreateFolder(folderName, parentPath, null);
			}
			catch(Exception ){}
		}

		public void CreateReport(string reportName, string parentPath, byte[] definition, string dataSourcePath)
		{
			Property[] reportProperties = new Property[1];
			Property hidden = new Property();
			hidden.Name = "Hidden";
			hidden.Value = reportName.StartsWith("_") ? "true" : "false";
			reportProperties[0] = hidden;
			_reportService.CreateReport(reportName, parentPath, true, definition, reportProperties);

			DataSourceReference dsRef = new DataSourceReference();
			dsRef.Reference = dataSourcePath;
			DataSource[] Sources = _reportService.GetItemDataSources(parentPath + "/" + reportName);
			if (Sources != null && Sources.Length > 0)
			{
				Sources[0].Item = dsRef;
				try
				{
					_reportService.SetItemDataSources(parentPath + "/" + reportName, Sources);
				}
				catch (Exception )
				{
					
				}
			}
		}

		public byte[] GetReportDefinition(string reportPath)
		{
			return _reportService.GetReportDefinition(reportPath);
		}
	}
}
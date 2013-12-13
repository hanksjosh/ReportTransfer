using ReportTransfer.ReportService;

namespace ReportTransfer
{
	public interface IRSCommunicator
	{
		CatalogItem[] GetExistingReports(string folder);
		void DeleteAllReports(string folder);
		void CreateFolder(string folderName, string parentPath);
		void CreateReport(string reportName, string parentPath, byte[] definition, string dataSourcePath);
		byte[] GetReportDefinition(string reportPath);
	}
}
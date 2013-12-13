using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReportTransfer.ReportService;

namespace ReportTransfer
{
	public class RSRepository
	{
		private IRSCommunicator _sourceCommunicator;
		private IRSCommunicator _destCommunicator;

		public RSRepository(IRSCommunicator sourceSourceCommunicator, IRSCommunicator destCommunicator)
		{
			_destCommunicator = destCommunicator;
			_sourceCommunicator = sourceSourceCommunicator;
		}

		public Folder GetExistingItems(string folder)
		{
			CatalogItem[] items = _sourceCommunicator.GetExistingReports(folder);
			Folder rootFolder = new Folder();
			rootFolder.Name = folder;
			var itemsList = items.ToList();
			List<Report> reports = new List<Report>();
			List<Folder> folders = new List<Folder>();
			folders.Add(rootFolder);
			foreach (var item in itemsList)
			{
				if (item.Type == ItemTypeEnum.Report)
				{
					var r = new Report(item);
					reports.Add(r);
				}
				else if (item.Type == ItemTypeEnum.Folder)
				{
					var f = new Folder(item);
					folders.Add(f);
				}
			}
			foreach (var currentFolder in folders.Where(f=>f.Depth > 1).OrderBy(f=>f.Depth))
			{
				FindParentFolder(currentFolder, rootFolder);
			}
			foreach (var report in reports)
			{
				FindParentFolder(report, rootFolder);
			}
			return rootFolder;
		}

		private void FindParentFolder(Report report, Folder rootFolder)
		{
			string[] path = report.Path.Split(new []{"/"},StringSplitOptions.RemoveEmptyEntries);
			Folder currentFolder = rootFolder;
			for (int i = 1; i < path.Length-1; i++)
			{
				string currentPath = path[i];
				var tempFolder = currentFolder.SubFolders.FirstOrDefault(f => f.Name.Equals(currentPath, StringComparison.OrdinalIgnoreCase));
				if (tempFolder != null)
				{
					currentFolder = tempFolder;
				}
			}
			report.ParentFolder = currentFolder;
			currentFolder.Reports.Add(report);
		}
		private void FindParentFolder(Folder folder, Folder rootFolder)
		{
			string[] path = folder.Path.Split(new []{"/"},StringSplitOptions.RemoveEmptyEntries);
			Folder currentFolder = rootFolder;
			for (int i = 1; i < path.Length-1; i++)
			{
				string currentPath = path[i];
				var tempFolder = currentFolder.SubFolders.FirstOrDefault(f => f.Name.Equals(currentPath, StringComparison.OrdinalIgnoreCase));
				if (tempFolder != null)
				{
					currentFolder = tempFolder;
				}
			}
			folder.ParentFolder = currentFolder;
			currentFolder.SubFolders.Add(folder);

		}


		public void UploadReports(Folder rootFolder, string newRootFolderName, string dataSourcePath)
		{
			rootFolder.Name = newRootFolderName;
			rootFolder.Path = "/" + newRootFolderName;
			UploadReportsForFolder(rootFolder, "/", newRootFolderName, dataSourcePath);
		}

		private void UploadReportsForFolder(Folder parentFolder, string rootPath, string newRootFolderName, string dataSourcePath)
		{
			parentFolder.SetNewRootPath(newRootFolderName);
			_destCommunicator.CreateFolder(parentFolder.Name, rootPath);
			foreach (var report in parentFolder.Reports.Where(r=>r.Selected))
			{
				var reportDefinition = _sourceCommunicator.GetReportDefinition(report.Path);
				_destCommunicator.CreateReport(report.Name, parentFolder.Path, reportDefinition, dataSourcePath);
			}
			foreach (var subFolder in parentFolder.SubFolders.Where(f=>f.ShouldTransfer))
			{
				UploadReportsForFolder(subFolder, parentFolder.Path, newRootFolderName, dataSourcePath);
			}
		}
	}
}

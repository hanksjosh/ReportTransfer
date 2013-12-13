using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ReportTransfer;
using ReportTransfer.ReportService;

namespace ReportTransferTests
{
	[TestClass]
	public class RSRepositoryTests
	{
		[TestMethod]
		public void GetExistingItemsTest()
		{
			Mock<IRSCommunicator> communicator = new Mock<IRSCommunicator>();
			CatalogItem item = new CatalogItem()
			{
				Path = "/folder/report",
				Type = ItemTypeEnum.Report,
				Name = "report"
			};
			communicator.Setup(c => c.GetExistingReports("folder")).Returns(new [] {item});

			RSRepository repo = new RSRepository(communicator.Object, null);
			Folder rootFolder = repo.GetExistingItems("folder");
			Assert.AreEqual("folder", rootFolder.Name);
			Assert.AreEqual("report", rootFolder.Reports.First().Name);

		}


		[TestMethod]
		public void GetExistingItemsWithSubFolder()
		{
			Mock<IRSCommunicator> communicator = new Mock<IRSCommunicator>();
			CatalogItem item = new CatalogItem()
			{
				Path = "/Folder/report",
				Type = ItemTypeEnum.Report,
				Name = "report"
			};
			CatalogItem subfolder = new CatalogItem()
			{
				Path = "/Folder/subfolder",
				Type = ItemTypeEnum.Folder,
				Name = "subfolder"
			};
			CatalogItem subItem = new CatalogItem()
			{
				Path = "/Folder/subfolder/subreport",
				Type = ItemTypeEnum.Report,
				Name = "subreport"
			};

			communicator.Setup(c => c.GetExistingReports("Folder")).Returns(new[] { item, subfolder, subItem });

			RSRepository repo = new RSRepository(communicator.Object, null);
			Folder rootFolder = repo.GetExistingItems("Folder");
			Assert.AreEqual("Folder", rootFolder.Name);
			Assert.AreEqual("report", rootFolder.Reports.First().Name);
			Folder subFolder = rootFolder.SubFolders.First();
			Assert.AreEqual("subfolder", subFolder.Name);
			Assert.AreEqual("subreport", subFolder.Reports.First().Name);

		}

		[TestMethod]
		public void GetExistingItemsWithSubSubFolder()
		{
			Mock<IRSCommunicator> communicator = new Mock<IRSCommunicator>();
			CatalogItem item = new CatalogItem()
			{
				Path = "/Folder/report",
				Type = ItemTypeEnum.Report,
				Name = "report"
			};
			CatalogItem subfolder = new CatalogItem()
			{
				Path = "/Folder/subfolder",
				Type = ItemTypeEnum.Folder,
				Name = "subfolder"
			};
			CatalogItem subItem = new CatalogItem()
			{
				Path = "/Folder/subfolder/subreport",
				Type = ItemTypeEnum.Report,
				Name = "subreport"
			};

			CatalogItem subsubfolder = new CatalogItem()
			{
				Path = "/Folder/subfolder/subsub",
				Type = ItemTypeEnum.Folder,
				Name = "subsub"
			};
			CatalogItem subsubItem = new CatalogItem()
			{
				Path = "/Folder/subfolder/subsub/subsubreport",
				Type = ItemTypeEnum.Report,
				Name = "subsubreport"
			};

			communicator.Setup(c => c.GetExistingReports("Folder")).Returns(new[] { item, subfolder, subItem, subsubfolder, subsubItem });

			RSRepository repo = new RSRepository(communicator.Object, null);
			Folder rootFolder = repo.GetExistingItems("Folder");
			Assert.AreEqual("Folder", rootFolder.Name);
			Assert.AreEqual("report", rootFolder.Reports.First().Name);
			Folder subFolder = rootFolder.SubFolders.First();
			Assert.AreEqual("subfolder", subFolder.Name);
			Assert.AreEqual("subreport", subFolder.Reports.First().Name);
			Folder subSubFolder = subFolder.SubFolders.First();
			Assert.AreEqual("subsub", subSubFolder.Name);
			Assert.AreEqual("subsubreport", subSubFolder.Reports.First().Name);

		}

		[TestMethod]
		public void UploadReports_SingleReport()
		{
			Folder rootFolder = new Folder() {Name = "RootFolder", Path = "/RootFolder"};
			Report report = new Report(){Name="Report", ParentFolder = rootFolder, Path = "/RootFolder/Report"};
			rootFolder.Reports.Add(report);

			Mock<IRSCommunicator> sourceComm = new Mock<IRSCommunicator>();
			sourceComm.Setup(s => s.GetReportDefinition("/RootFolder/Report")).Returns(new byte[0]);

			Mock<IRSCommunicator> destComm = new Mock<IRSCommunicator>();

			RSRepository repo = new RSRepository(sourceComm.Object, destComm.Object);

			repo.UploadReports(rootFolder, "NewRootFolder", "dsPath");

			sourceComm.Verify(s=>s.GetReportDefinition("/RootFolder/Report"), Times.Once());

			destComm.Verify(d=>d.CreateFolder("NewRootFolder", "/"), Times.Once());
			destComm.Verify(d => d.CreateReport("Report", "/NewRootFolder", It.IsAny<byte[]>(), "dsPath"), Times.Once());
		}

		[TestMethod]
		public void UploadReports_SingleReport_IgnoresUnselected()
		{
			Folder rootFolder = new Folder() {Name = "RootFolder", Path = "/RootFolder"};
			Report report = new Report(){Name="Report", ParentFolder = rootFolder, Path = "/RootFolder/Report"};
			Report report2 = new Report(){Name="Report2", ParentFolder = rootFolder, Path = "/RootFolder/Report2", Selected = false};
			rootFolder.Reports.Add(report);
			rootFolder.Reports.Add(report2);

			Mock<IRSCommunicator> sourceComm = new Mock<IRSCommunicator>();
			sourceComm.Setup(s => s.GetReportDefinition("/RootFolder/Report")).Returns(new byte[0]);

			Mock<IRSCommunicator> destComm = new Mock<IRSCommunicator>();

			RSRepository repo = new RSRepository(sourceComm.Object, destComm.Object);

			repo.UploadReports(rootFolder, "NewRootFolder", "dsPath");

			sourceComm.Verify(s=>s.GetReportDefinition("/RootFolder/Report"), Times.Once());

			destComm.Verify(d=>d.CreateFolder("NewRootFolder", "/"), Times.Once());
			destComm.Verify(d => d.CreateReport("Report", "/NewRootFolder", It.IsAny<byte[]>(), "dsPath"), Times.Once());
			destComm.Verify(d => d.CreateReport("Report2", "/NewRootFolder", It.IsAny<byte[]>(), "dsPath"), Times.Never());
		}

		[TestMethod]
		public void UploadReports_SubfolderWithReport()
		{
			Folder rootFolder = new Folder() {Name = "RootFolder", Path = "/RootFolder"};
			Report report = new Report(){Name="Report", ParentFolder = rootFolder, Path = "/RootFolder/Report"};
			rootFolder.Reports.Add(report);
			Folder subFolder = new Folder(){Name="SubFolder", Path="/RootFolder/SubFolder"};
			Report subReport = new Report() { Name = "SubReport", ParentFolder = subFolder, Path = "/RootFolder/SubFolder/SubReport" };
			subFolder.Reports.Add(subReport);
			rootFolder.SubFolders.Add(subFolder);

			Mock<IRSCommunicator> sourceComm = new Mock<IRSCommunicator>();
			sourceComm.Setup(s => s.GetReportDefinition("/RootFolder/Report")).Returns(new byte[0]);

			Mock<IRSCommunicator> destComm = new Mock<IRSCommunicator>();

			RSRepository repo = new RSRepository(sourceComm.Object, destComm.Object);

			repo.UploadReports(rootFolder, "NewRootFolder", "dsPath");

			sourceComm.Verify(s=>s.GetReportDefinition("/RootFolder/Report"), Times.Once());
			sourceComm.Verify(s=>s.GetReportDefinition("/RootFolder/SubFolder/SubReport"), Times.Once());

			destComm.Verify(d=>d.CreateFolder("NewRootFolder", "/"), Times.Once());
			destComm.Verify(d => d.CreateReport("Report", "/NewRootFolder", It.IsAny<byte[]>(), "dsPath"), Times.Once());
			destComm.Verify(d => d.CreateFolder("SubFolder", "/NewRootFolder"), Times.Once());
			destComm.Verify(d => d.CreateReport("SubReport", "/NewRootFolder/SubFolder", It.IsAny<byte[]>(), "dsPath"), Times.Once());
		}
		[TestMethod]
		public void UploadReports_SubfolderWithReport_IgnoresUnselected()
		{
			Folder rootFolder = new Folder() {Name = "RootFolder", Path = "/RootFolder"};
			Report report = new Report(){Name="Report", ParentFolder = rootFolder, Path = "/RootFolder/Report"};
			rootFolder.Reports.Add(report);
			Folder subFolder = new Folder(){Name="SubFolder", Path="/RootFolder/SubFolder", Selected =false};
			Report subReport = new Report() { Name = "SubReport", ParentFolder = subFolder, Path = "/RootFolder/SubFolder/SubReport", Selected = false};
			subFolder.Reports.Add(subReport);
			rootFolder.SubFolders.Add(subFolder);

			Mock<IRSCommunicator> sourceComm = new Mock<IRSCommunicator>();
			sourceComm.Setup(s => s.GetReportDefinition("/RootFolder/Report")).Returns(new byte[0]);

			Mock<IRSCommunicator> destComm = new Mock<IRSCommunicator>();

			RSRepository repo = new RSRepository(sourceComm.Object, destComm.Object);

			repo.UploadReports(rootFolder, "NewRootFolder", "dsPath");

			sourceComm.Verify(s=>s.GetReportDefinition("/RootFolder/Report"), Times.Once());
			sourceComm.Verify(s=>s.GetReportDefinition("/RootFolder/SubFolder/SubReport"), Times.Never());

			destComm.Verify(d=>d.CreateFolder("NewRootFolder", "/"), Times.Once());
			destComm.Verify(d => d.CreateReport("Report", "/NewRootFolder", It.IsAny<byte[]>(), "dsPath"), Times.Once());
			destComm.Verify(d => d.CreateFolder("SubFolder", "/NewRootFolder"), Times.Never());
			destComm.Verify(d => d.CreateReport("SubReport", "/NewRootFolder/SubFolder", It.IsAny<byte[]>(), "dsPath"), Times.Never());
		}

		[TestMethod]
		public void UploadReports_SubfolderWithReport_FolderUnselectedButChildReportSelected()
		{
			Folder rootFolder = new Folder() {Name = "RootFolder", Path = "/RootFolder"};
			Report report = new Report(){Name="Report", ParentFolder = rootFolder, Path = "/RootFolder/Report"};
			rootFolder.Reports.Add(report);
			Folder subFolder = new Folder(){Name="SubFolder", Path="/RootFolder/SubFolder", Selected =false};
			Report subReport = new Report() { Name = "SubReport", ParentFolder = subFolder, Path = "/RootFolder/SubFolder/SubReport", Selected = true};
			subFolder.Reports.Add(subReport);
			rootFolder.SubFolders.Add(subFolder);

			Mock<IRSCommunicator> sourceComm = new Mock<IRSCommunicator>();
			sourceComm.Setup(s => s.GetReportDefinition("/RootFolder/Report")).Returns(new byte[0]);

			Mock<IRSCommunicator> destComm = new Mock<IRSCommunicator>();

			RSRepository repo = new RSRepository(sourceComm.Object, destComm.Object);

			repo.UploadReports(rootFolder, "NewRootFolder", "dsPath");

			sourceComm.Verify(s=>s.GetReportDefinition("/RootFolder/Report"), Times.Once());
			sourceComm.Verify(s=>s.GetReportDefinition("/RootFolder/SubFolder/SubReport"), Times.Once());

			destComm.Verify(d=>d.CreateFolder("NewRootFolder", "/"), Times.Once());
			destComm.Verify(d => d.CreateReport("Report", "/NewRootFolder", It.IsAny<byte[]>(), "dsPath"), Times.Once());
			destComm.Verify(d => d.CreateFolder("SubFolder", "/NewRootFolder"), Times.Once());
			destComm.Verify(d => d.CreateReport("SubReport", "/NewRootFolder/SubFolder", It.IsAny<byte[]>(), "dsPath"), Times.Once());
		}
	}
}

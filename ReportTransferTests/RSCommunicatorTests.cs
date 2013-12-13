using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReportTransfer;

namespace ReportTransferTests
{
	[TestClass]
	public class RSCommunicatorTests
	{
		[TestMethod]
		public void GetExistingReports()
		{
			RSCommunicator com = new RSCommunicator("http://devdb/reportserver/reportservice2005.asmx");
			var result = com.GetExistingReports("Josh");
			int i = 0;
		}
	}
}

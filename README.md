ReportTransfer is a tool for copying Sql Reporting Services Reports to a different server, or a different folder within the same server. 

Executable can be dowloaded from: https://skydrive.live.com/redir?resid=69D774D9604C3FB7!3554&authkey=!AN9GYh7L7DySeX4&ithint=folder%2c.exe

Usage:
To copy reports from an existig Reporting Services instance to a new location:

1. Fill out the ReportService URL of the Report Server where the reports are located.

2. Enter the Folder on that Report Server where the reports are located.

3. Click the Get Reports Button.  This will query the Report Server, and show the available reports in the tree view on the right.

4. Select the reports that you desire to copy.

5. Fill out the ReportService URL of the Report Server where you want to copy the reports to.

6. Enter the Folder that the reports will be copied to. If this folder does not exist, it will be created. Any reports already in this folder that match the name of a report being copied will be overwritten.

7. Fill out the DataSource field. Once the reports are copied, the report will be pointed to the specified DataSource.

8. Click Copy Reports.

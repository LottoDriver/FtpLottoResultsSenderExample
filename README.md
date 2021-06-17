# LottoDriver FtpLottoResults Sender Example

This repository contains examples for LottoDriver FTP LottoResults Sender.

If you are targeting **.NET Framework**, start with **LottoDriver.FtpLottoResultsSenderExample.NetFramework.sln** solution file. If you are using Visual Studio IDE, VS2017 or later is required.

If you are targeting **.NET Core**, start with **LottoDriver.Examples.NetCore.sln** solution file. If you are using Visual Studio IDE, VS2019 or later is required.

The examples include:

- WindowsService - Demonstrates simple implementation of LottoDriver FTP LottoResults Sender as a WindowsService. If you are targeting .NET Framework in your applications, start here.
- WorkerService - Demonstrates simple implementation of LottoDriver FTP LottoResults Sender as a WorkerService. If you are targeting .NET Core in your applications, start here.

## Quick Start

### 1. Implement **ILottoResultsDataProvider** to be able to get Lotto Results from your storage of choice

In this example, default implementation of **ILottoResultsDataProvider** interface is using hardcoded data. You will need to provide new implementation or update existing, to retrieve real data from your database or some other source.

### 2. Obtain FTP account info (Host and UserName/Password)

To run the applications you need to obtain FTP account info from LottoDriver. The easiest way to obtain it is by contacting: <info@lottodriver.com>.
When you receive the credentials enter them in one of the configuration files (depending on which example you are using):

- `LottoDriver.FtpLottoResultsSenderExample.WinService\App.config`
- `LottoDriver.FtpLottoResultsSenderExample.WorkerService\appsettings.json`

In the meantime, LottoDriver team will configure result processor on our side, so we can import data into our database.

### 3. Run WinService or WorkerService

After setting the configuration, simply start the application of your choice (WinService or WorkerService) from Visual Studio.

### 4. Observe the Data

If everything is correctly configured in previous steps, you should see logs printed in the console window with related information.

using FluentFTP;
using LottoDriver.FtpLottoResultsSenderExample.Common.FileManager;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LottoDriver.FtpLottoResultsSenderExample.Common
{
    public class FtpFileUploadManager : IFtpFileUploadManager
    {
        private readonly ILogger<FtpFileUploadManager> _logger;
        private readonly ILocalResultsFileManager _localResultsFileManager;
        private readonly Func<IFtpClient> _ftpClientFactory;

        public FtpFileUploadManager(ILogger<FtpFileUploadManager> logger,
            ILocalResultsFileManager localResultsFileManager,
            Func<IFtpClient> ftpClientFactory
            )
        {
            _logger = logger;
            _localResultsFileManager = localResultsFileManager;
            _ftpClientFactory = ftpClientFactory;
        }

        /// <inheritdoc />
        public async Task UploadFiles(CancellationToken cancellationToken)
        {
            _logger.LogInformation("FtpFileUploadManager.UploadFiles() - START");

            var filesToUpload = _localResultsFileManager.GetFiles();

            using (var client = _ftpClientFactory())
            {
                _logger.LogInformation("Creating FTP client.");
                await client.ConnectAsync(cancellationToken);

                foreach (var file in filesToUpload)
                {
                    try
                    {
                        var remotePath = $"/{new FileInfo(file).Name}";
                        _logger.LogInformation($"Uploading file. Filename: '{file}'.");

                        var status = await client.UploadFileAsync(localPath: file, remotePath: remotePath, verifyOptions: FtpVerify.Throw, token: cancellationToken);

                        if (status != FtpStatus.Success)
                        {
                            _logger.LogWarning($"File transfer is not successful. Filename: '{file}'.");
                            _logger.LogInformation($"Continuing with next file if any is available...");
                            continue;
                        }

                        _logger.LogInformation($"Moving file to backup. Filename: '{file}'.");
                        _localResultsFileManager.MoveToBackupDirectory(file);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Error uploading file. Filename: '{file}'.");
                    }
                }

                _localResultsFileManager.DeleteOldFiles();
            }

            _logger.LogInformation("FtpFileUploadManager.UploadFiles() - END");
        }
    }
}

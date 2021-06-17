using LottoDriver.FtpLottoResultsSenderExample.Common;
using LottoDriver.FtpLottoResultsSenderExample.Common.FileManager;
using LottoDriver.FtpLottoResultsSenderExample.Common.LottoResultsDataProvider;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LottoDriver.FtpLottoResultsSenderExample.WorkerService
{
    public class FtpWorker : BackgroundService
    {
        private const int DelayMs = 30_000;
        private readonly ILogger<FtpWorker> _logger;
        private readonly IFtpFileUploadManager _fileUploadManager;
        private readonly ILocalResultsFileManager _localResultsFileManager;
        private readonly ILottoResultsDataProvider _lottoResultsDataProvider;

        public FtpWorker(ILogger<FtpWorker> logger, IFtpFileUploadManager fileUploadManager,
            ILocalResultsFileManager localResultsFileManager,
            ILottoResultsDataProvider lottoResultsDataProvider)
        {
            _logger = logger;
            _fileUploadManager = fileUploadManager;
            _localResultsFileManager = localResultsFileManager;
            _lottoResultsDataProvider = lottoResultsDataProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("FtpWorker started at: {time}", DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Getting results data...");
                    var data = _lottoResultsDataProvider.GetLottoResultsData();
                    
                    _logger.LogInformation("Writing results data to file...");
                    _localResultsFileManager.CreateFile(data);

                    _logger.LogInformation("Uploading results data file to LottoDriver FTP server...");
                    await _fileUploadManager.UploadFiles(stoppingToken);

                    _logger.LogInformation($"Delay processing for {TimeSpan.FromMilliseconds(DelayMs).TotalSeconds} seconds...");
                    await Task.Delay(DelayMs, stoppingToken);
                }
                catch (TaskCanceledException ex)
                {
                    if (stoppingToken.IsCancellationRequested)
                    {
                        _logger.LogWarning($"Stopping {nameof(FtpWorker)}");
                    }
                    else
                    {
                        _logger.LogError(ex, $"Error in {nameof(FtpWorker)}: {ex.Message}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error in {nameof(FtpWorker)}: {ex.Message}");
                }
            }
        }
    }
}

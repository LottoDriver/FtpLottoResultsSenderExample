using FluentFTP;
using LottoDriver.FtpLottoResultsSenderExample.Common;
using LottoDriver.FtpLottoResultsSenderExample.Common.FileManager;
using LottoDriver.FtpLottoResultsSenderExample.Common.LottoResultsDataProvider;
using Serilog;
using System;
using System.Configuration;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace LottoDriver.Examples.CustomersApi.WinService
{
    public partial class FtpWorkerService : ServiceBase
    {
        private const int DelayMs = 30_000;
        private static bool _stateRunning = false;
        private static CancellationTokenSource _tokenSource;
        private Task _task;
        private CancellationToken _token;

        private readonly ILottoResultsDataProvider _lottoResultsDataProvider;
        private readonly ILocalResultsFileManager _localResultsFileManager;
        private readonly IFtpFileUploadManager _fileUploadManager;

        public FtpWorkerService()
        {
            InitializeComponent();

            _lottoResultsDataProvider = new LottoResultsDataProvider();

            _localResultsFileManager = new LocalResultsFileManager(
                ConfigurationManager.AppSettings["AppSettings:LocalInputDirectoryFullPath"],
                ConfigurationManager.AppSettings["AppSettings:LocalBackupDirectoryFullPath"],
                ConfigurationManager.AppSettings["AppSettings:ClientName"]);

            _fileUploadManager = new FtpFileUploadManager(new SerilogTypedLogger<FtpFileUploadManager>(Serilog.Log.Logger),
                _localResultsFileManager,
                () =>
                {
                    var client = new FtpClient(
                        ConfigurationManager.AppSettings["AppSettings:FtpHost"],
                        ConfigurationManager.AppSettings["AppSettings:FtpUsername"],
                        ConfigurationManager.AppSettings["AppSettings:FtpPassword"]
                    )
                    {
                        EncryptionMode = FtpEncryptionMode.Explicit,
                        ValidateAnyCertificate = true
                    };
                    return client;
                });
        }

        protected override void OnStart(string[] args)
        {
            Log.Information("FtpWorkerService started at: {time}", DateTimeOffset.Now);

            #region Old code
            //while (!_cancellationTokenSource.Token.IsCancellationRequested)
            //{
            //    try
            //    {
            //        Log.Information("Getting results data...");
            //        var data = _lottoResultsDataProvider.GetLottoResultsData();
            //
            //        Log.Information("Writing results data to file...");
            //        _localResultsFileManager.CreateFile(data);
            //
            //        Log.Information("Uploading results data file to LottoDriver FTP server...");
            //        _fileUploadManager.UploadFiles(_cancellationTokenSource.Token).GetAwaiter().GetResult();
            //
            //        Log.Information($"Delay processing for {TimeSpan.FromMilliseconds(DelayMs).TotalSeconds} seconds...");
            //        Task.Delay(DelayMs, _cancellationTokenSource.Token).GetAwaiter().GetResult();
            //    }
            //    catch (TaskCanceledException ex)
            //    {
            //        if (_cancellationTokenSource.Token.IsCancellationRequested)
            //        {
            //            Log.Warning($"Stopping {nameof(FtpWorkerService)}");
            //        }
            //        else
            //        {
            //            Log.Error(ex, $"Error in {nameof(FtpWorkerService)}: {ex.Message}");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Log.Error(ex, $"Error in {nameof(FtpWorkerService)}: {ex.Message}");
            //    }
            //} 
            #endregion

            if (!_stateRunning)
            {
                _stateRunning = true;
                _tokenSource = new CancellationTokenSource();
                _token = _tokenSource.Token;

                try
                {
                    _task = Task.Factory.StartNew(() => DoWorkAsync(_token), _token);
                    Log.Information($"Task {_task.Id} executing...");
                    _task.Wait();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Error in OnStart - Exception {nameof(FtpWorkerService)}: {ex.Message}");
                }
            }
        }

        protected override void OnStop()
        {
            Log.Information($"Stopping {nameof(FtpWorkerService)}");

            if (_stateRunning)
            {
                try
                {
                    if (_tokenSource != null)
                    {
                        _tokenSource.Cancel();
                        Log.Information("Task cancellation requested.");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Error in OnStop - Exception {nameof(FtpWorkerService)}: {ex.Message}");
                }
                finally
                {
                    Log.Information("OnStop - finally");
                    if (_tokenSource != null)
                    {
                        _tokenSource.Dispose();
                        Log.Information("TokenSource Disposed");
                    }
                }
            }
        }

        public async Task<bool> DoWorkAsync(CancellationToken cancellationToken)
        {
            Log.Information("DoWorkAsync() starting...");
            
            // Already canceled?
            cancellationToken.ThrowIfCancellationRequested();

            while (!cancellationToken.IsCancellationRequested)
            {
                Log.Information("DoWorkAsync() method cycle started.");
                try
                {
                    Log.Information("Getting results data...");
                    var data = _lottoResultsDataProvider.GetLottoResultsData();

                    Log.Information("Writing results data to file...");
                    _localResultsFileManager.CreateFile(data);

                    Log.Information("Uploading results data file to LottoDriver FTP server...");
                    await _fileUploadManager.UploadFiles(cancellationToken);

                    Log.Information($"Delay processing for {TimeSpan.FromMilliseconds(DelayMs).TotalSeconds} seconds...");
                    await Task.Delay(DelayMs, cancellationToken);
                }
                catch (TaskCanceledException ex)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Log.Warning($"Stopping {nameof(FtpWorkerService)}");
                    }
                    else
                    {
                        Log.Error(ex, $"Error in {nameof(FtpWorkerService)}: {ex.Message}");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Error in {nameof(FtpWorkerService)}: {ex.Message}");
                }

                // Time to stop?
                if (cancellationToken.IsCancellationRequested)
                {
                    Log.Information("Task cancelled...");
                    _stateRunning = false;
                }
            }

            return true;
        }

        public void ConsoleStart(string[] args)
        {
            OnStart(args);
        }
    }
}

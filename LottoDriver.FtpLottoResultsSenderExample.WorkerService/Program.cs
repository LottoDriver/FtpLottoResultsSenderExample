using FluentFTP;
using LottoDriver.FtpLottoResultsSenderExample.Common;
using LottoDriver.FtpLottoResultsSenderExample.Common.FileManager;
using LottoDriver.FtpLottoResultsSenderExample.Common.LottoResultsDataProvider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace LottoDriver.FtpLottoResultsSenderExample.WorkerService
{
    internal static class Program
    {
        public static int Main(string[] args)
        {
            if (!CreateLogger()) return 1;

            try
            {
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Host terminated unexpectedly!");
                return 2;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseWindowsService()
                .ConfigureAppConfiguration((hostContext, builder) =>
                {
                    if (hostContext.HostingEnvironment.IsDevelopment())
                    {
                        builder.AddUserSecrets<FtpWorker>();
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;

                    services.AddTransient<ILottoResultsDataProvider, LottoResultsDataProvider>();
                    services.AddTransient<IFtpFileUploadManager, FtpFileUploadManager>();
                    services.AddTransient<ILocalResultsFileManager>(_ =>
                        new LocalResultsFileManager(configuration.GetValue<string>("AppSettings:LocalInputDirectoryFullPath"),
                            configuration.GetValue<string>("AppSettings:LocalBackupDirectoryFullPath"),
                            configuration.GetValue<string>("AppSettings:ClientName")));

                    services.AddTransient<Func<IFtpClient>>(x => () =>
                    {
                        var client = new FtpClient(
                            configuration.GetValue<string>("AppSettings:FtpHost"),
                            configuration.GetValue<string>("AppSettings:FtpUsername"),
                            configuration.GetValue<string>("AppSettings:FtpPassword")
                        )
                        {
                            EncryptionMode = FtpEncryptionMode.Explicit,
                            ValidateAnyCertificate = true
                    };


                        return client;
                    });

                    services.AddHostedService<FtpWorker>();
                });
        }

        private static bool CreateLogger()
        {
            try
            {
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json", optional: true)
                    .Build();

                Log.Logger = new LoggerConfiguration()
                    .Enrich.WithMachineName()
                    .Enrich.WithProcessId()
                    .ReadFrom.Configuration(configuration)
                    .CreateLogger();

                return true;
            }
            catch (Exception ex)
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine("Logger creation failed!");
                    Console.WriteLine(ex.Message);
                }

                return false;
            }
        }
    }
}
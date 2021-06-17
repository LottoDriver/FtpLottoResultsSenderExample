using Serilog;
using System;
using System.ServiceProcess;

namespace LottoDriver.Examples.CustomersApi.WinService
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .CreateLogger();

            var svc = new FtpWorkerService();

            if (Environment.UserInteractive)
            {
                Console.WriteLine($"Starting {nameof(FtpWorkerService)} example service");
                svc.ConsoleStart(args);
                Console.WriteLine("Started! Press Enter to stop.");

                Console.ReadLine();

                Console.WriteLine($"Stopping {nameof(FtpWorkerService)} example service");
                svc.Stop();
                Console.WriteLine("Stopped.");
            }
            else
            {
                ServiceBase.Run(svc);
            }

            Log.CloseAndFlush();
        }
    }
}

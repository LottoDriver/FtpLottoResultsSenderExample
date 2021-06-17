using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;
using System;

namespace LottoDriver.Examples.CustomersApi.WinService
{

    public class SerilogTypedLogger<T> : ILogger<T>
    {
        private readonly ILogger _logger;

        public SerilogTypedLogger(Serilog.ILogger logger)
        {
            using (var logfactory = new SerilogLoggerFactory(logger))
                _logger = logfactory.CreateLogger(typeof(T).FullName);
        }

        IDisposable ILogger.BeginScope<TState>(TState state) =>
            _logger.BeginScope<TState>(state);

        bool ILogger.IsEnabled(LogLevel logLevel) =>
            _logger.IsEnabled(logLevel);

        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) =>
            _logger.Log<TState>(logLevel, eventId, state, exception, formatter);
    }
}

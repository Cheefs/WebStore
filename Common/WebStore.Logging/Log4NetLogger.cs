﻿using System.ComponentModel;
using System.Reflection;
using System.Xml;
using log4net;
using Microsoft.Extensions.Logging;

namespace WebStore.Logging;

public class Log4NetLogger : ILogger
{
    private readonly ILog _log;

    public Log4NetLogger(string catagory, XmlElement config)
    {
        var loggerRepository = LogManager
           .CreateRepository(
                Assembly.GetEntryAssembly(),
                typeof(log4net.Repository.Hierarchy.Hierarchy));

        _log = LogManager.GetLogger(loggerRepository.Name, catagory);

        log4net.Config.XmlConfigurator.Configure(config);
    }

    public IDisposable BeginScope<TState>(TState state) => null!;

    public bool IsEnabled(LogLevel level) => level switch
    {
        LogLevel.None => false,
        LogLevel.Trace => _log.IsDebugEnabled,
        LogLevel.Debug => _log.IsDebugEnabled,
        LogLevel.Information => _log.IsInfoEnabled,
        LogLevel.Warning => _log.IsWarnEnabled,
        LogLevel.Error => _log.IsErrorEnabled,
        LogLevel.Critical => _log.IsFatalEnabled,
        _ => throw new InvalidEnumArgumentException(nameof(level), (int)level, typeof(LogLevel))
    };

    public void Log<TState>(
        LogLevel level,
        EventId id,
        TState state,
        Exception? error,
        Func<TState, Exception?, string> Formatter)
    {
        if (Formatter is null) throw new ArgumentNullException(nameof(Formatter));

        if (!IsEnabled(level))
            return;

        var log_string = Formatter(state, error);
        if (string.IsNullOrWhiteSpace(log_string) && error is null)
            return;

        switch (level)
        {
            default: throw new InvalidEnumArgumentException(nameof(level), (int)level, typeof(LogLevel));

            case LogLevel.None: break;

            case LogLevel.Trace:
            case LogLevel.Debug:
                _log.Debug(log_string);
                break;

            case LogLevel.Information:
                _log.Info(log_string);
                break;

            case LogLevel.Warning:
                _log.Warn(log_string);
                break;

            case LogLevel.Error:
                _log.Error(log_string, error);
                break;

            case LogLevel.Critical:
                _log.Fatal(log_string, error);
                break;
        }
    }
}
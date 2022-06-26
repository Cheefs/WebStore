using System.Collections.Concurrent;
using System.Xml;
using Microsoft.Extensions.Logging;

namespace WebStore.Logging;

public class Log4NetLoggerProvider : ILoggerProvider
{
    private readonly string _configurationFile;
    private readonly ConcurrentDictionary<string, Log4NetLogger> _loggers = new();

    public Log4NetLoggerProvider(string configurationFile) => _configurationFile = configurationFile;

    public ILogger CreateLogger(string Category) => _loggers.GetOrAdd(
        Category,
        (category, config) =>
        {
            var xml = new XmlDocument();
            xml.Load(config);
            return new Log4NetLogger(category, xml["log4net"] ?? throw new InvalidOperationException("Не удалось извлечь из xml-документа элемент log4net"));
        },
        _configurationFile);

    public void Dispose() => _loggers.Clear();
}
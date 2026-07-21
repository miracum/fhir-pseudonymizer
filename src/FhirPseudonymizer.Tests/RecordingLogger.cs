using Microsoft.Extensions.Logging;

namespace FhirPseudonymizer.Tests;

/// <summary>
///     Records what a component writes to the log, rendered the way a provider would render it.
///     A fake cannot answer that question: the message only exists once the formatter has run
///     over the state, which is exactly where an injected line break would show up.
/// </summary>
internal sealed class RecordingLogger<T> : ILogger<T>
{
    public List<string> Messages { get; } = [];

    public IDisposable BeginScope<TState>(TState state)
        where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception exception,
        Func<TState, Exception, string> formatter
    )
    {
        Messages.Add(formatter(state, exception));
    }
}

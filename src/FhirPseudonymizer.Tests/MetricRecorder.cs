using System.Collections.Concurrent;
using System.Diagnostics.Metrics;

namespace FhirPseudonymizer.Tests;

/// <summary>
///     Collects what one of the app's instruments records, but only from the async flow that
///     created the recorder: the instruments are static, so tests running in parallel record to
///     them as well. A <see cref="MeterListener" /> callback runs synchronously on the recording
///     thread, which carries the recording code's <see cref="AsyncLocal{T}" /> values.
/// </summary>
internal sealed class MetricRecorder<T> : IDisposable
    where T : struct
{
    private readonly object scope;
    private readonly MeterListener listener = new();
    private readonly ConcurrentQueue<RecordedMeasurement<T>> measurements = new();

    public MetricRecorder(string instrumentName)
    {
        // Set synchronously, so it is seen by everything the calling test awaits afterwards - and
        // shared by all recorders that test creates.
        scope = MetricRecorderScope.Current.Value ??= new object();

        listener.InstrumentPublished = (instrument, meterListener) =>
        {
            if (instrument.Meter.Name == "FhirPseudonymizer" && instrument.Name == instrumentName)
            {
                meterListener.EnableMeasurementEvents(instrument);
            }
        };
        listener.SetMeasurementEventCallback<T>(
            (_, value, tags, _) =>
            {
                if (!ReferenceEquals(MetricRecorderScope.Current.Value, scope))
                {
                    return;
                }

                var tagDictionary = new Dictionary<string, object>();
                foreach (var tag in tags)
                {
                    tagDictionary[tag.Key] = tag.Value;
                }

                measurements.Enqueue(new RecordedMeasurement<T>(value, tagDictionary));
            }
        );
        listener.Start();
    }

    public IReadOnlyList<RecordedMeasurement<T>> Measurements => [.. measurements];

    public void Dispose()
    {
        listener.Dispose();
    }
}

internal static class MetricRecorderScope
{
    public static AsyncLocal<object> Current { get; } = new();
}

internal sealed record RecordedMeasurement<T>(T Value, IReadOnlyDictionary<string, object> Tags);

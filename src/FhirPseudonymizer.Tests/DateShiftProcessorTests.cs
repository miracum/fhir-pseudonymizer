using Hl7.Fhir.ElementModel;
using Hl7.Fhir.FhirPath;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.Anonymizer.Core.Extensions;
using Microsoft.Health.Fhir.Anonymizer.Core.Processors;

namespace FhirPseudonymizer.Tests;

public class DateShiftProcessorTests
{
    [Fact]
    public async Task Process_WithFixedOffsetInDays_ShiftsDateByExactAmount()
    {
        var processor = new DateShiftProcessor(
            dateShiftKey: "test-key",
            dateShiftKeyPrefix: "test-prefix",
            enablePartialDatesForRedact: false
        );

        var patient = new Patient { BirthDate = "1990-01-15" };
        var node = ElementNode.FromElement(patient.ToTypedElement());
        var birthDateNode = node.Children("birthDate").CastElementNodes().First();

        var settings = new Dictionary<string, object>
        {
            { "dateShiftFixedOffsetInDays", new Integer(30) },
        };

        await processor.ProcessAsync(birthDateNode, settings: settings);

        birthDateNode.Value.ToString().Should().Be("1990-02-14");
    }

    [Fact]
    public async Task Process_WithNegativeFixedOffsetInDays_ShiftsDateBackward()
    {
        var processor = new DateShiftProcessor(
            dateShiftKey: "test-key",
            dateShiftKeyPrefix: "test-prefix",
            enablePartialDatesForRedact: false
        );

        var patient = new Patient { BirthDate = "1990-01-15" };
        var node = ElementNode.FromElement(patient.ToTypedElement());
        var birthDateNode = node.Children("birthDate").CastElementNodes().First();

        var settings = new Dictionary<string, object>
        {
            { "dateShiftFixedOffsetInDays", new Integer(-10) },
        };

        await processor.ProcessAsync(birthDateNode, settings: settings);

        birthDateNode.Value.ToString().Should().Be("1990-01-05");
    }

    [Fact]
    public async Task Process_WithZeroFixedOffsetInDays_KeepsDateUnchanged()
    {
        var processor = new DateShiftProcessor(
            dateShiftKey: "test-key",
            dateShiftKeyPrefix: "test-prefix",
            enablePartialDatesForRedact: false
        );

        var patient = new Patient { BirthDate = "1990-01-15" };
        var node = ElementNode.FromElement(patient.ToTypedElement());
        var birthDateNode = node.Children("birthDate").CastElementNodes().First();

        var settings = new Dictionary<string, object>
        {
            { "dateShiftFixedOffsetInDays", new Integer(0) },
        };

        await processor.ProcessAsync(birthDateNode, settings: settings);

        birthDateNode.Value.ToString().Should().Be("1990-01-15");
    }

    [Fact]
    public async Task Process_WithoutFixedOffset_UsesHashBasedOffset()
    {
        // Hash-based offset depends on dateShiftKey + dateShiftKeyPrefix.
        // Different prefixes should produce different results.
        var processor1 = new DateShiftProcessor(
            dateShiftKey: "test-key",
            dateShiftKeyPrefix: "prefix-A",
            enablePartialDatesForRedact: false
        );

        var processor2 = new DateShiftProcessor(
            dateShiftKey: "test-key",
            dateShiftKeyPrefix: "prefix-B",
            enablePartialDatesForRedact: false
        );

        var patient1 = new Patient { BirthDate = "1990-01-15" };
        var node1 = ElementNode.FromElement(patient1.ToTypedElement());
        var birthDateNode1 = node1.Children("birthDate").CastElementNodes().First();

        var patient2 = new Patient { BirthDate = "1990-01-15" };
        var node2 = ElementNode.FromElement(patient2.ToTypedElement());
        var birthDateNode2 = node2.Children("birthDate").CastElementNodes().First();

        await processor1.ProcessAsync(birthDateNode1, settings: null);
        await processor2.ProcessAsync(birthDateNode2, settings: null);

        // Different prefixes should yield different shifted dates (hash-based behavior)
        birthDateNode1.Value.ToString().Should().NotBe(birthDateNode2.Value.ToString());
    }

    [Fact]
    public async Task Process_WithFixedOffsetOnDateTime_ShiftsByExactAmount()
    {
        var processor = new DateShiftProcessor(
            dateShiftKey: "test-key",
            dateShiftKeyPrefix: "test-prefix",
            enablePartialDatesForRedact: false
        );

        // Use Condition.recordedDate which is a non-polymorphic dateTime field
        var condition = new Condition { RecordedDate = "2020-06-15T10:30:00+02:00" };
        var node = ElementNode.FromElement(condition.ToTypedElement());
        var recordedDateNode = node.Children("recordedDate").CastElementNodes().First();

        var settings = new Dictionary<string, object>
        {
            { "dateShiftFixedOffsetInDays", new Integer(5) },
        };

        await processor.ProcessAsync(recordedDateNode, settings: settings);

        // Date shifted by 5 days, time zeroed out per existing behavior
        recordedDateNode.Value.ToString().Should().Be("2020-06-20T00:00:00+02:00");
    }

    [Fact]
    public async Task Process_WithIntValueInsteadOfFhirInteger_ShiftsDateCorrectly()
    {
        var processor = new DateShiftProcessor(
            dateShiftKey: "test-key",
            dateShiftKeyPrefix: "test-prefix",
            enablePartialDatesForRedact: false
        );

        var patient = new Patient { BirthDate = "1990-01-15" };
        var node = ElementNode.FromElement(patient.ToTypedElement());
        var birthDateNode = node.Children("birthDate").CastElementNodes().First();

        // Using raw int instead of FHIR Integer
        var settings = new Dictionary<string, object> { { "dateShiftFixedOffsetInDays", 30 } };

        await processor.ProcessAsync(birthDateNode, settings: settings);

        birthDateNode.Value.ToString().Should().Be("1990-02-14");
    }
}

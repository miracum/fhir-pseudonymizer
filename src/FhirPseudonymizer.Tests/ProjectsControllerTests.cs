using FhirPseudonymizer.Controllers;
using FhirPseudonymizer.Projects;

namespace FhirPseudonymizer.Tests;

public class ProjectsControllerTests
{
    [Fact]
    public async Task Register_WithANameCarryingALineBreak_ShouldLogItOnASingleLine()
    {
        // A name is logged before anything has vetted it, so a caller could otherwise forge a
        // second log entry by putting %0A in the route.
        var logger = new RecordingLogger<ProjectsController>();
        var controller = new ProjectsController(A.Fake<IProjectRegistry>(), logger);

        await controller.Register("innocent\nERROR: forged log entry");

        logger.Messages.Should().ContainSingle().Which.Should().NotContainAny("\n", "\r");
    }

    [Fact]
    public async Task Register_WithAFloodingName_ShouldLogATruncatedName()
    {
        // Nothing bounds the name before the length check rejects it, so logging it whole hands
        // a caller a way to push the whole route into one log line.
        var logger = new RecordingLogger<ProjectsController>();
        var controller = new ProjectsController(A.Fake<IProjectRegistry>(), logger);

        await controller.Register(new string('a', 10_000));

        var message = logger.Messages.Should().ContainSingle().Subject;
        message
            .Length.Should()
            .BeLessThan(200, "a rejected name belongs in the log only far enough to recognise it");
    }
}

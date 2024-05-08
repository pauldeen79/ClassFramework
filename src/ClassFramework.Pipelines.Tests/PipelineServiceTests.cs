namespace ClassFramework.Pipelines.Tests;

public class PipelineServiceTests : TestBase<PipelineService>
{
    [Fact]
    public async Task Process_Returns_Invalid_When_Pipeline_Returns_Builder_With_ValidationErrors()
    {
        // Arrange
        var pipeline = Fixture.Freeze<IPipeline<BuilderContext>>();
        // note that by doing nothing on the received builder in the builder context, the name will be empty, and this is a required field.
        // thus, we are creating an invalid result 8-)
        pipeline.Process(Arg.Any<BuilderContext>(), Arg.Any<CancellationToken>()).Returns(x => Result.Success());
        var sut = CreateSut();
        var sourceModel = CreateModel();
        var settings = CreateSettingsForBuilder().Build();
        var context = new BuilderContext(sourceModel, settings, CultureInfo.InvariantCulture);

        // Act
        var result = await sut.Process(context);

        // Assert
        result.Status.Should().Be(ResultStatus.Invalid);
    }
}

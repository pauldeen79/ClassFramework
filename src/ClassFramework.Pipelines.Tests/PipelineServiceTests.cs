namespace ClassFramework.Pipelines.Tests;

public class PipelineServiceTests : TestBase<PipelineService>
{
    public class Process_BuilderContext : PipelineServiceTests
    {
        [Fact]
        public async Task Returns_Invalid_When_Pipeline_Returns_Builder_With_ValidationErrors()
        {
            // Arrange
            var pipeline = Fixture.Freeze<IPipeline<BuilderContext>>();
            // note that by doing nothing on the received builder in the builder context, the name will be empty, and this is a required field.
            // thus, we are creating an invalid result 8-)
            pipeline.ProcessAsync(Arg.Any<BuilderContext>(), Arg.Any<CancellationToken>()).Returns(x => Result.Success());
            var sut = CreateSut();
            var sourceModel = CreateClass();
            var settings = CreateSettingsForBuilder();
            var context = new BuilderContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Invalid);
        }

        [Fact]
        public async Task Returns_InnerResult_When_Pipeline_Returns_NonSuccesful_Result()
        {
            // Arrange
            var pipeline = Fixture.Freeze<IPipeline<BuilderContext>>();
            pipeline.ProcessAsync(Arg.Any<BuilderContext>(), Arg.Any<CancellationToken>()).Returns(x => Result.Error("Kaboom!"));
            var sut = CreateSut();
            var sourceModel = CreateClass();
            var settings = CreateSettingsForBuilder();
            var context = new BuilderContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Error);
            result.ErrorMessage.ShouldBe("Kaboom!");
            result.Value.ShouldBeNull();
        }

        [Fact]
        public async Task Returns_InnerResult_With_Value_When_Pipeline_Returns_Succesful_Result()
        {
            // Arrange
            var pipeline = Fixture.Freeze<IPipeline<BuilderContext>>();
            pipeline
                .ProcessAsync(Arg.Any<BuilderContext>(), Arg.Any<CancellationToken>())
                .Returns(x =>
                {
                    x.ArgAt<BuilderContext>(0).Builder.WithName("MyClass").WithNamespace("MyNamespace");
                    return Result.Success("Kaboom!");
                });
            var sut = CreateSut();
            var sourceModel = CreateClass();
            var settings = CreateSettingsForBuilder();
            var context = new BuilderContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            result.Value.ShouldNotBeNull();
            result.Value!.Name.ShouldBe("MyClass");
            result.Value!.Namespace.ShouldBe("MyNamespace");
        }
    }
}

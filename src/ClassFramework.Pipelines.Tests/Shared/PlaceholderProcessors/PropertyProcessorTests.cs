namespace ClassFramework.Pipelines.Tests.Shared.PlaceholderProcessors;

public class PropertyProcessorTests : TestBase<PropertyProcessor>
{
    public class Process : PropertyProcessorTests
    {
        private static Property CreateModel() => new PropertyBuilder().WithName("Delegate").WithType(typeof(List<string>)).Build();

        [Fact]
        public void Returns_Continue_When_Context_Is_Not_ParentChildContext()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.Process("Placeholder", CultureInfo.InvariantCulture, null, Fixture.Freeze<IFormattableStringParser>());

            // Assert
            result.Status.Should().Be(ResultStatus.Continue);
        }

        [Fact]
        public void Returns_Continue_On_Unknown_Value()
        {
            // Arrange
            var sut = CreateSut();
            var context = new PipelineContext<Property>(CreateModel());

            // Act
            var result = sut.Process("Placeholder", CultureInfo.InvariantCulture, context, Fixture.Freeze<IFormattableStringParser>());

            // Assert
            result.Status.Should().Be(ResultStatus.Continue);
        }
    }
}

namespace ClassFramework.Pipelines.Tests.Shared.PlaceholderProcessors;

public class TypeProcessorTests : TestBase<TypeProcessor>
{
    public class Process : TypeProcessorTests
    {
        private Type Model { get; }

        public Process()
        {
            Model = typeof(SomeClass);
        }

        [Fact]
        public void Returns_Continue_When_Context_Is_Not_PipelineContext()
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
            var context = new PipelineContext<Type, BuilderContext>(Model, new BuilderContext(CreateClass(), new PipelineSettingsBuilder().Build(), CultureInfo.InvariantCulture));

            // Act
            var result = sut.Process("Placeholder", CultureInfo.InvariantCulture, context, Fixture.Freeze<IFormattableStringParser>());

            // Assert
            result.Status.Should().Be(ResultStatus.Continue);
        }

        [Theory]
        [InlineData("GenericArgumentsWithBrackets", "")]
        [InlineData("GenericArgumentsWithoutBrackets", "")]
        [InlineData("Class.GenericArgumentsWithBrackets", "")]
        [InlineData("Class.GenericArgumentsWithoutBrackets", "")]
        public void Returns_Ok_With_Correct_Value_On_Known_Value(string value, string expectedValue)
        {
            // Arrange
            var sut = CreateSut();
            var context = new PipelineContext<Type>(Model);

            // Act
            var result = sut.Process(value, CultureInfo.InvariantCulture, context, Fixture.Freeze<IFormattableStringParser>());

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value!.ToString().Should().Be(expectedValue);
        }
    }
}

#pragma warning disable CA1040 // Avoid empty interfaces
public interface IMyInterface { }
#pragma warning restore CA1040 // Avoid empty interfaces
#pragma warning disable S2094 // Classes should not be empty
public class SomeClass { }
#pragma warning restore S2094 // Classes should not be empty

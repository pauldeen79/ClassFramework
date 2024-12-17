﻿namespace ClassFramework.Pipelines.Tests.Shared.PlaceholderProcessors;

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

        [Theory]
        [InlineData("TypeName.CollectionItemType.GenericArgumentsWithBrackets", "")]
        [InlineData("TypeName.CollectionItemType.GenericArgumentsWithoutBrackets", "")]
        public void Returns_Ok_With_Correct_Value_On_Known_Value(string value, string expectedValue)
        {
            // Arrange
            var formattableStringParser = InitializeParser();
            var sut = CreateSut();
            var settings = new PipelineSettingsBuilder().Build();
            var model = CreateModel();
            var context = new PropertyContext(model, settings, CultureInfo.InvariantCulture, model.TypeName, string.Empty);

            // Act
            var result = sut.Process(value, CultureInfo.InvariantCulture, context, formattableStringParser);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value!.ToString().Should().Be(expectedValue);
        }
    }
}

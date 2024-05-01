namespace ClassFramework.Pipelines.Tests.Reflection.PlaceholderProcessors;

public class ReflectionPipelinePlaceholderProcessorTests : TestBase<ReflectionPipelinePlaceholderProcessor>
{
    public class Process : ReflectionPipelinePlaceholderProcessorTests
    {
        private Property CreatePropertyModel(bool isNullable = false) => new PropertyBuilder().WithName("Delegate").WithType(typeof(List<string>)).WithIsNullable(isNullable).Build();
        private ClassBuilder CreateModel() => new ClassBuilder().WithName("MyClass").WithNamespace("MyNamespace");

        [Fact]
        public void Throws_On_Null_FormattableStringParser()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            sut.Invoking(x => x.Process("Placeholder", CultureInfo.InvariantCulture, null, formattableStringParser: null!))
               .Should().Throw<ArgumentNullException>().WithParameterName("formattableStringParser");
        }

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
        public void Returns_Result_From_PropertyPlaceholderProcessor_On_Unknown_Value()
        {
            // Arrange
            var sourceModel = typeof(MyClass);
            var propertyPlaceholderProcessor = Fixture.Freeze<IPipelinePlaceholderProcessor>();
            var externalResult = Result.NoContent<FormattableStringParserResult>();
            propertyPlaceholderProcessor.Process(Arg.Any<string>(), Arg.Any<IFormatProvider>(), Arg.Any<object?>(), Arg.Any<IFormattableStringParser>()).Returns(externalResult);
            var sut = CreateSut();
            var settings = CreateSettingsForReflection();
            var context = new ParentChildContext<PipelineContext<ReflectionContext>, Property>(new PipelineContext<ReflectionContext>(new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture)), CreatePropertyModel(), settings.Build());

            // Act
            var result = sut.Process("Placeholder", CultureInfo.InvariantCulture, context, Fixture.Freeze<IFormattableStringParser>());

            // Assert
            result.Should().BeSameAs(externalResult);
        }

        [Fact]
        public void Returns_Result_When_PipelinePlaceholderProcessor_Supports_The_Value_With_ParentChildContext()
        {
            // Arrange
            var sourceModel = typeof(MyClass);
            var pipelinePlaceholderProcessor = Fixture.Freeze<IPipelinePlaceholderProcessor>();
            pipelinePlaceholderProcessor.Process("Value", Arg.Any<IFormatProvider>(), Arg.Any<object?>(), Arg.Any<IFormattableStringParser>()).Returns(Result.Success<FormattableStringParserResult>("MyResult"));
            var sut = CreateSut();
            var settings = CreateSettingsForReflection();
            var context = new ParentChildContext<PipelineContext<ReflectionContext>, Property>(new PipelineContext<ReflectionContext>(new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture)), CreatePropertyModel(), settings.Build());

            // Act
            var result = sut.Process("Value", CultureInfo.InvariantCulture, context, Fixture.Freeze<IFormattableStringParser>());

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value!.ToString().Should().Be("MyResult");
        }

        [Fact]
        public void Returns_Continue_When_PipelinePlaceholderProcessor_Does_Not_Support_The_Value_With_ParentChildContext()
        {
            // Arrange
            var sourceModel = typeof(MyClass);
            var pipelinePlaceholderProcessor = Fixture.Freeze<IPipelinePlaceholderProcessor>();
            pipelinePlaceholderProcessor.Process("Value", Arg.Any<IFormatProvider>(), Arg.Any<object?>(), Arg.Any<IFormattableStringParser>()).Returns(Result.Continue<FormattableStringParserResult>());
            var sut = CreateSut();
            var settings = CreateSettingsForReflection();
            var context = new ParentChildContext<PipelineContext<ReflectionContext>, Property>(new PipelineContext<ReflectionContext>(new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture)), CreatePropertyModel(), settings.Build());

            // Act
            var result = sut.Process("Value", CultureInfo.InvariantCulture, context, Fixture.Freeze<IFormattableStringParser>());

            // Assert
            result.Status.Should().Be(ResultStatus.Continue);
        }

        [Fact]
        public void Returns_Result_When_PipelinePlaceholderProcessor_Supports_The_Value_With_PipelineChildContext()
        {
            // Arrange
            var sourceModel = typeof(MyClass);
            var pipelinePlaceholderProcessor = Fixture.Freeze<IPipelinePlaceholderProcessor>();
            pipelinePlaceholderProcessor.Process("Value", Arg.Any<IFormatProvider>(), Arg.Any<object?>(), Arg.Any<IFormattableStringParser>()).Returns(Result.Success<FormattableStringParserResult>("MyResult"));
            var sut = CreateSut();
            var context = new PipelineContext<ReflectionContext>(new ReflectionContext(sourceModel, CreateSettingsForReflection().Build(), CultureInfo.InvariantCulture));

            // Act
            var result = sut.Process("Value", CultureInfo.InvariantCulture, context, Fixture.Freeze<IFormattableStringParser>());

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value!.ToString().Should().Be("MyResult");
        }

        [Fact]
        public void Returns_Continue_When_PipelinePlaceholderProcessor_Does_Not_Support_The_Value_With_PipelineChildContext()
        {
            // Arrange
            var sourceModel = typeof(MyClass);
            var pipelinePlaceholderProcessor = Fixture.Freeze<IPipelinePlaceholderProcessor>();
            pipelinePlaceholderProcessor.Process("Value", Arg.Any<IFormatProvider>(), Arg.Any<object?>(), Arg.Any<IFormattableStringParser>()).Returns(Result.Continue<FormattableStringParserResult>());
            var sut = CreateSut();
            var context = new PipelineContext<ReflectionContext>(new ReflectionContext(sourceModel, CreateSettingsForReflection().Build(), CultureInfo.InvariantCulture));

            // Act
            var result = sut.Process("Value", CultureInfo.InvariantCulture, context, Fixture.Freeze<IFormattableStringParser>());

            // Assert
            result.Status.Should().Be(ResultStatus.Continue);
        }
    }
}

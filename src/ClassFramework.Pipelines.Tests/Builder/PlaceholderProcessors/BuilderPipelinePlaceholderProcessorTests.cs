namespace ClassFramework.Pipelines.Tests.Builder.PlaceholderProcessors;

public class BuilderPipelinePlaceholderProcessorTests : TestBase<BuilderPipelinePlaceholderProcessor>
{
    public class Process : BuilderPipelinePlaceholderProcessorTests
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
            var propertyPlaceholderProcessor = Fixture.Freeze<IPipelinePlaceholderProcessor>();
            var externalResult = Result.NoContent<FormattableStringParserResult>();
            propertyPlaceholderProcessor.Process(Arg.Any<string>(), Arg.Any<IFormatProvider>(), Arg.Any<object?>(), Arg.Any<IFormattableStringParser>()).Returns(externalResult);
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder();
            var context = new ParentChildContext<PipelineContext<BuilderContext>, Property>(new PipelineContext<BuilderContext>(new BuilderContext(CreateModel().Build(), settings.Build(), CultureInfo.InvariantCulture)), CreatePropertyModel(), settings.Build());

            // Act
            var result = sut.Process("Placeholder", CultureInfo.InvariantCulture, context, Fixture.Freeze<IFormattableStringParser>());

            // Assert
            result.Should().BeSameAs(externalResult);
        }

        [Theory]
        [InlineData("NullCheck.Source.Argument", "if (source.Delegate is not null) ")] // null checks are enabled in this unit test
        [InlineData("NullCheck.Argument", "if (@delegate is null) throw new System.ArgumentNullException(nameof(@delegate));")] // null checks are enabled in this unit test
        [InlineData("BuildersNamespace", "MyNamespace.Builders")]
        public void Returns_Ok_With_Correct_Value_On_Known_Value_With_ParentChildContext_With_NullChecks_Enabled(string value, string expectedValue)
        {
            // Arrange
            var formattableStringParser = Fixture.Freeze<IFormattableStringParser>();
            formattableStringParser.Parse("{$class.Namespace}.Builders", Arg.Any<FormattableStringParserSettings>(), Arg.Any<object?>()).Returns(Result.Success<FormattableStringParserResult>("MyNamespace.Builders"));
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(addNullChecks: true, validateArguments: ArgumentValidationType.None);
            var context = new ParentChildContext<PipelineContext<BuilderContext>, Property>(new PipelineContext<BuilderContext>(new BuilderContext(CreateModel().Build(), settings.Build(), CultureInfo.InvariantCulture)), CreatePropertyModel(), settings.Build());

            // Act
            var result = sut.Process(value, CultureInfo.InvariantCulture, context, Fixture.Freeze<IFormattableStringParser>());

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value!.ToString().Should().Be(expectedValue);
        }

        [Theory]
        [InlineData("NullCheck.Source.Argument", "")]
        public void Returns_Ok_With_Correct_Value_On_Known_Value_With_ParentChildContext_With_NullChecks_Enabled_SourceEntity_Already_Validated(string value, string expectedValue)
        {
            // Arrange
            var formattableStringParser = Fixture.Freeze<IFormattableStringParser>();
            formattableStringParser.Parse("{$class.Namespace}.Builders", Arg.Any<FormattableStringParserSettings>(), Arg.Any<object?>()).Returns(Result.Success<FormattableStringParserResult>("MyNamespace.Builders"));
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(addNullChecks: true, validateArguments: ArgumentValidationType.IValidatableObject);
            var context = new ParentChildContext<PipelineContext<BuilderContext>, Property>(new PipelineContext<BuilderContext>(new BuilderContext(CreateModel().Build(), settings.Build(), CultureInfo.InvariantCulture)), CreatePropertyModel(), settings.Build());

            // Act
            var result = sut.Process(value, CultureInfo.InvariantCulture, context, Fixture.Freeze<IFormattableStringParser>());

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value!.ToString().Should().Be(expectedValue);
        }

        [Theory]
        [InlineData("NullCheck.Source.Argument", "")]
        [InlineData("NullCheck.Argument", "")]
        public void Returns_Ok_With_Correct_Value_On_Known_Value_With_ParentChildContext_Without_NullChecks(string value, string expectedValue)
        {
            // Arrange
            var formattableStringParser = Fixture.Freeze<IFormattableStringParser>();
            formattableStringParser.Parse("{$class.Namespace}.Builders", Arg.Any<FormattableStringParserSettings>(), Arg.Any<object?>()).Returns(Result.Success<FormattableStringParserResult>("MyNamespace.Builders"));
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(addNullChecks: false);
            var context = new ParentChildContext<PipelineContext<BuilderContext>, Property>(new PipelineContext<BuilderContext>(new BuilderContext(CreateModel().Build(), settings.Build(), CultureInfo.InvariantCulture)), CreatePropertyModel(), settings.Build());

            // Act
            var result = sut.Process(value, CultureInfo.InvariantCulture, context, Fixture.Freeze<IFormattableStringParser>());

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value!.ToString().Should().Be(expectedValue);
        }

        [Theory]
        [InlineData("BuildersNamespace", "MyNamespace.Builders")]
        public void Returns_Ok_With_Correct_Value_On_Known_Value_With_PipelineContext(string value, string expectedValue)
        {
            // Arrange
            var formattableStringParser = Fixture.Freeze<IFormattableStringParser>();
            formattableStringParser.Parse("{$class.Namespace}.Builders", Arg.Any<FormattableStringParserSettings>(), Arg.Any<object?>()).Returns(Result.Success<FormattableStringParserResult>("MyNamespace.Builders"));
            var sut = CreateSut();
            var context = new PipelineContext<BuilderContext>(new BuilderContext(CreateModel().Build(), CreateSettingsForBuilder(addNullChecks: true).Build(), CultureInfo.InvariantCulture));

            // Act
            var result = sut.Process(value, CultureInfo.InvariantCulture, context, Fixture.Freeze<IFormattableStringParser>());

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value!.ToString().Should().Be(expectedValue);
        }

        [Theory]
        [InlineData("NullableRequiredSuffix", true, "!")]
        [InlineData("NullableRequiredSuffix", false, "")]
        [InlineData("NullableSuffix", true, "?")]
        [InlineData("NullableSuffix", false, "")]
        public void Returns_Ok_With_Correct_Value_On_Known_Value_Depending_On_IsNullable(string value, bool isNullable, string expectedResult)
        {
            // Arrange
            var formattableStringParser = Fixture.Freeze<IFormattableStringParser>();
            formattableStringParser.Parse("{$class.Namespace}.Builders", Arg.Any<FormattableStringParserSettings>(), Arg.Any<object?>()).Returns(Result.Success<FormattableStringParserResult>("MyNamespace.Builders"));
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(enableNullableReferenceTypes: true);
            var context = new ParentChildContext<PipelineContext<BuilderContext>, Property>(new PipelineContext<BuilderContext>(new BuilderContext(CreateModel().Build(), settings.Build(), CultureInfo.InvariantCulture)), CreatePropertyModel(isNullable), settings.Build());

            // Act
            var result = sut.Process(value, CultureInfo.InvariantCulture, context, Fixture.Freeze<IFormattableStringParser>());

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value!.ToString().Should().Be(expectedResult);
        }

        [Fact]
        public void Returns_Result_When_PipelinePlaceholderProcessor_Supports_The_Value_With_ParentChildContext()
        {
            // Arrange
            var pipelinePlaceholderProcessor = Fixture.Freeze<IPipelinePlaceholderProcessor>();
            pipelinePlaceholderProcessor.Process("Value", Arg.Any<IFormatProvider>(), Arg.Any<object?>(), Arg.Any<IFormattableStringParser>()).Returns(Result.Success<FormattableStringParserResult>("MyResult"));
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(addNullChecks: true);
            var context = new ParentChildContext<PipelineContext<BuilderContext>, Property>(new PipelineContext<BuilderContext>(new BuilderContext(CreateModel().Build(), settings.Build(), CultureInfo.InvariantCulture)), CreatePropertyModel(), settings.Build());

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
            var pipelinePlaceholderProcessor = Fixture.Freeze<IPipelinePlaceholderProcessor>();
            pipelinePlaceholderProcessor.Process("Value", Arg.Any<IFormatProvider>(), Arg.Any<object?>(), Arg.Any<IFormattableStringParser>()).Returns(Result.Continue<FormattableStringParserResult>());
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(addNullChecks: true);
            var context = new ParentChildContext<PipelineContext<BuilderContext>, Property>(new PipelineContext<BuilderContext>(new BuilderContext(CreateModel().Build(), settings.Build(), CultureInfo.InvariantCulture)), CreatePropertyModel(), settings.Build());

            // Act
            var result = sut.Process("Value", CultureInfo.InvariantCulture, context, Fixture.Freeze<IFormattableStringParser>());

            // Assert
            result.Status.Should().Be(ResultStatus.Continue);
        }

        [Fact]
        public void Returns_Result_When_PipelinePlaceholderProcessor_Supports_The_Value_With_PipelineChildContext()
        {
            // Arrange
            var pipelinePlaceholderProcessor = Fixture.Freeze<IPipelinePlaceholderProcessor>();
            pipelinePlaceholderProcessor.Process("Value", Arg.Any<IFormatProvider>(), Arg.Any<object?>(), Arg.Any<IFormattableStringParser>()).Returns(Result.Success<FormattableStringParserResult>("MyResult"));
            var sut = CreateSut();
            var context = new PipelineContext<BuilderContext>(new BuilderContext(CreateModel().Build(), CreateSettingsForBuilder(addNullChecks: true).Build(), CultureInfo.InvariantCulture));

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
            var pipelinePlaceholderProcessor = Fixture.Freeze<IPipelinePlaceholderProcessor>();
            pipelinePlaceholderProcessor.Process("Value", Arg.Any<IFormatProvider>(), Arg.Any<object?>(), Arg.Any<IFormattableStringParser>()).Returns(Result.Continue<FormattableStringParserResult>());
            var sut = CreateSut();
            var context = new PipelineContext<BuilderContext>(new BuilderContext(CreateModel().Build(), CreateSettingsForBuilder(addNullChecks: true).Build(), CultureInfo.InvariantCulture));

            // Act
            var result = sut.Process("Value", CultureInfo.InvariantCulture, context, Fixture.Freeze<IFormattableStringParser>());

            // Assert
            result.Status.Should().Be(ResultStatus.Continue);
        }
    }
}

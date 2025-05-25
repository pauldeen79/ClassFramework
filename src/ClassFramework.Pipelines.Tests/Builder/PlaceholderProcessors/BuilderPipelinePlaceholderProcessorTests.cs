//namespace ClassFramework.Pipelines.Tests.Builder.PlaceholderProcessors;

//public class BuilderPipelinePlaceholderProcessorTests : TestBase<BuilderPipelinePlaceholderProcessor>
//{
//    public class Evaluate : BuilderPipelinePlaceholderProcessorTests
//    {
//        private static Property CreatePropertyModel(bool isNullable = false) => new PropertyBuilder().WithName("Delegate").WithType(typeof(List<string>)).WithIsNullable(isNullable).Build();
//        private static ClassBuilder CreateModel() => new ClassBuilder().WithName("MyClass").WithNamespace("MyNamespace");

//        [Fact]
//        public void Throws_On_Null_FormattableStringParser()
//        {
//            // Arrange
//            var sut = CreateSut();

//            // Act & Assert
//            Action a = () => sut.Evaluate("Placeholder", new PlaceholderSettingsBuilder(), null, formattableStringParser: null!);
//            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("formattableStringParser");
//        }

//        [Fact]
//        public void Returns_Continue_When_Context_Is_Not_ParentChildContext()
//        {
//            // Arrange
//            var sut = CreateSut();

//            // Act
//            var result = sut.Evaluate("Placeholder", new PlaceholderSettingsBuilder(), null, Fixture.Freeze<IFormattableStringParser>());

//            // Assert
//            result.Status.ShouldBe(ResultStatus.Continue);
//        }

//        [Fact]
//        public void Returns_Result_From_PropertyPlaceholderProcessor_On_Unknown_Value()
//        {
//            // Arrange
//            var propertyPlaceholderProcessor = Fixture.Freeze<IPipelinePlaceholderProcessor>();
//            var externalResult = Result.NoContent<GenericFormattableString>();
//            propertyPlaceholderProcessor.Evaluate(Arg.Any<string>(), Arg.Any<PlaceholderSettings>(), Arg.Any<object?>(), Arg.Any<IFormattableStringParser>()).Returns(externalResult);
//            var sut = CreateSut();
//            var settings = CreateSettingsForBuilder();
//            var context = new ParentChildContext<PipelineContext<BuilderContext>, Property>(new PipelineContext<BuilderContext>(new BuilderContext(CreateModel().Build(), settings, CultureInfo.InvariantCulture)), CreatePropertyModel(), settings);

//            // Act
//            var result = sut.Evaluate("Placeholder", new PlaceholderSettingsBuilder(), context, Fixture.Freeze<IFormattableStringParser>());

//            // Assert
//            result.ShouldBeSameAs(externalResult);
//        }

//        [Theory]
//        [InlineData("NullCheck.Source.Argument", "if (source.Delegate is not null) ")] // null checks are enabled in this unit test
//        [InlineData("NullCheck.Argument", "if (@delegate is null) throw new System.ArgumentNullException(nameof(@delegate));")] // null checks are enabled in this unit test
//        [InlineData("BuildersNamespace", "MyNamespace.Builders")]
//        public void Returns_Ok_With_Correct_Value_On_Known_Value_With_ParentChildContext_With_NullChecks_Enabled(string value, string expectedValue)
//        {
//            // Arrange
//            var formattableStringParser = Fixture.Freeze<IFormattableStringParser>();
//            formattableStringParser.Parse("{$class.Namespace}.Builders", Arg.Any<FormattableStringParserSettings>(), Arg.Any<object?>()).Returns(Result.Success<GenericFormattableString>("MyNamespace.Builders"));
//            var sut = CreateSut();
//            var settings = CreateSettingsForBuilder(addNullChecks: true, validateArguments: ArgumentValidationType.None);
//            var context = new ParentChildContext<PipelineContext<BuilderContext>, Property>(new PipelineContext<BuilderContext>(new BuilderContext(CreateModel().Build(), settings, CultureInfo.InvariantCulture)), CreatePropertyModel(), settings);

//            // Act
//            var result = sut.Evaluate(value, new PlaceholderSettingsBuilder(), context, Fixture.Freeze<IFormattableStringParser>());

//            // Assert
//            result.Status.ShouldBe(ResultStatus.Ok);
//            result.Value!.ToString().ShouldBe(expectedValue);
//        }

//        [Theory]
//        [InlineData("NullCheck.Source.Argument", "")]
//        public void Returns_Ok_With_Correct_Value_On_Known_Value_With_ParentChildContext_With_NullChecks_Enabled_SourceEntity_Already_Validated(string value, string expectedValue)
//        {
//            // Arrange
//            var formattableStringParser = Fixture.Freeze<IFormattableStringParser>();
//            formattableStringParser.Parse("{$class.Namespace}.Builders", Arg.Any<FormattableStringParserSettings>(), Arg.Any<object?>()).Returns(Result.Success<GenericFormattableString>("MyNamespace.Builders"));
//            var sut = CreateSut();
//            var settings = CreateSettingsForBuilder(addNullChecks: true, validateArguments: ArgumentValidationType.IValidatableObject);
//            var context = new ParentChildContext<PipelineContext<BuilderContext>, Property>(new PipelineContext<BuilderContext>(new BuilderContext(CreateModel().Build(), settings, CultureInfo.InvariantCulture)), CreatePropertyModel(), settings);

//            // Act
//            var result = sut.Evaluate(value, new PlaceholderSettingsBuilder(), context, Fixture.Freeze<IFormattableStringParser>());

//            // Assert
//            result.Status.ShouldBe(ResultStatus.Ok);
//            result.Value!.ToString().ShouldBe(expectedValue);
//        }

//        [Theory]
//        [InlineData("NullCheck.Source.Argument", "")]
//        [InlineData("NullCheck.Argument", "")]
//        public void Returns_Ok_With_Correct_Value_On_Known_Value_With_ParentChildContext_Without_NullChecks(string value, string expectedValue)
//        {
//            // Arrange
//            var formattableStringParser = Fixture.Freeze<IFormattableStringParser>();
//            formattableStringParser.Parse("{$class.Namespace}.Builders", Arg.Any<FormattableStringParserSettings>(), Arg.Any<object?>()).Returns(Result.Success<GenericFormattableString>("MyNamespace.Builders"));
//            var sut = CreateSut();
//            var settings = CreateSettingsForBuilder(addNullChecks: false);
//            var context = new ParentChildContext<PipelineContext<BuilderContext>, Property>(new PipelineContext<BuilderContext>(new BuilderContext(CreateModel().Build(), settings, CultureInfo.InvariantCulture)), CreatePropertyModel(), settings);

//            // Act
//            var result = sut.Evaluate(value, new PlaceholderSettingsBuilder(), context, Fixture.Freeze<IFormattableStringParser>());

//            // Assert
//            result.Status.ShouldBe(ResultStatus.Ok);
//            result.Value!.ToString().ShouldBe(expectedValue);
//        }

//        [Theory]
//        [InlineData("BuildersNamespace", "MyNamespace.Builders")]
//        public void Returns_Ok_With_Correct_Value_On_Known_Value_With_PipelineContext(string value, string expectedValue)
//        {
//            // Arrange
//            var formattableStringParser = Fixture.Freeze<IFormattableStringParser>();
//            formattableStringParser.Parse("{$class.Namespace}.Builders", Arg.Any<FormattableStringParserSettings>(), Arg.Any<object?>()).Returns(Result.Success<GenericFormattableString>("MyNamespace.Builders"));
//            var sut = CreateSut();
//            var context = new PipelineContext<BuilderContext>(new BuilderContext(CreateModel().Build(), CreateSettingsForBuilder(addNullChecks: true).Build(), CultureInfo.InvariantCulture));

//            // Act
//            var result = sut.Evaluate(value, new PlaceholderSettingsBuilder(), context, Fixture.Freeze<IFormattableStringParser>());

//            // Assert
//            result.Status.ShouldBe(ResultStatus.Ok);
//            result.Value!.ToString().ShouldBe(expectedValue);
//        }

//        [Fact]
//        public void Returns_Result_When_PipelinePlaceholderProcessor_Supports_The_Value_With_ParentChildContext()
//        {
//            // Arrange
//            var pipelinePlaceholderProcessor = Fixture.Freeze<IPipelinePlaceholderProcessor>();
//            pipelinePlaceholderProcessor.Evaluate("Value", Arg.Any<PlaceholderSettings>(), Arg.Any<object?>(), Arg.Any<IFormattableStringParser>()).Returns(Result.Success<GenericFormattableString>("MyResult"));
//            var sut = CreateSut();
//            var settings = CreateSettingsForBuilder(addNullChecks: true);
//            var context = new ParentChildContext<PipelineContext<BuilderContext>, Property>(new PipelineContext<BuilderContext>(new BuilderContext(CreateModel().Build(), settings, CultureInfo.InvariantCulture)), CreatePropertyModel(), settings);

//            // Act
//            var result = sut.Evaluate("Value", new PlaceholderSettingsBuilder(), context, Fixture.Freeze<IFormattableStringParser>());

//            // Assert
//            result.Status.ShouldBe(ResultStatus.Ok);
//            result.Value!.ToString().ShouldBe("MyResult");
//        }

//        [Fact]
//        public void Returns_Continue_When_PipelinePlaceholderProcessor_Does_Not_Support_The_Value_With_ParentChildContext()
//        {
//            // Arrange
//            var pipelinePlaceholderProcessor = Fixture.Freeze<IPipelinePlaceholderProcessor>();
//            pipelinePlaceholderProcessor.Evaluate("Value", Arg.Any<PlaceholderSettings>(), Arg.Any<object?>(), Arg.Any<IFormattableStringParser>()).Returns(Result.Continue<GenericFormattableString>());
//            var sut = CreateSut();
//            var settings = CreateSettingsForBuilder(addNullChecks: true);
//            var context = new ParentChildContext<PipelineContext<BuilderContext>, Property>(new PipelineContext<BuilderContext>(new BuilderContext(CreateModel().Build(), settings, CultureInfo.InvariantCulture)), CreatePropertyModel(), settings);

//            // Act
//            var result = sut.Evaluate("Value", new PlaceholderSettingsBuilder(), context, Fixture.Freeze<IFormattableStringParser>());

//            // Assert
//            result.Status.ShouldBe(ResultStatus.Continue);
//        }

//        [Fact]
//        public void Returns_Result_When_PipelinePlaceholderProcessor_Supports_The_Value_With_PipelineChildContext()
//        {
//            // Arrange
//            var pipelinePlaceholderProcessor = Fixture.Freeze<IPipelinePlaceholderProcessor>();
//            pipelinePlaceholderProcessor.Evaluate("Value", Arg.Any<PlaceholderSettings>(), Arg.Any<object?>(), Arg.Any<IFormattableStringParser>()).Returns(Result.Success<GenericFormattableString>("MyResult"));
//            var sut = CreateSut();
//            var context = new PipelineContext<BuilderContext>(new BuilderContext(CreateModel().Build(), CreateSettingsForBuilder(addNullChecks: true).Build(), CultureInfo.InvariantCulture));

//            // Act
//            var result = sut.Evaluate("Value", new PlaceholderSettingsBuilder(), context, Fixture.Freeze<IFormattableStringParser>());

//            // Assert
//            result.Status.ShouldBe(ResultStatus.Ok);
//            result.Value!.ToString().ShouldBe("MyResult");
//        }

//        [Fact]
//        public void Returns_Continue_When_PipelinePlaceholderProcessor_Does_Not_Support_The_Value_With_PipelineChildContext()
//        {
//            // Arrange
//            var pipelinePlaceholderProcessor = Fixture.Freeze<IPipelinePlaceholderProcessor>();
//            pipelinePlaceholderProcessor.Evaluate("Value", Arg.Any<PlaceholderSettings>(), Arg.Any<object?>(), Arg.Any<IFormattableStringParser>()).Returns(Result.Continue<GenericFormattableString>());
//            var sut = CreateSut();
//            var context = new PipelineContext<BuilderContext>(new BuilderContext(CreateModel().Build(), CreateSettingsForBuilder(addNullChecks: true).Build(), CultureInfo.InvariantCulture));

//            // Act
//            var result = sut.Evaluate("Value", new PlaceholderSettingsBuilder(), context, Fixture.Freeze<IFormattableStringParser>());

//            // Assert
//            result.Status.ShouldBe(ResultStatus.Continue);
//        }
//    }
//}

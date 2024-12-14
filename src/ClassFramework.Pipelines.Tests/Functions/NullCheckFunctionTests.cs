namespace ClassFramework.Pipelines.Tests.Functions;

public class NullCheckFunctionTests : TestBase<NullCheckFunction>
{
    public class Parse : NullCheckFunctionTests
    {
        [Fact]
        public void Returns_Success_On_Context_Of_Type_ContextBase()
        {
            // Arrange
            InitializeParser();
            var functionParseResult = new FunctionParseResultBuilder()
                .WithFunctionName("NullCheck")
                .WithFormatProvider(CultureInfo.InvariantCulture)
                .Build();
            var context = new PropertyContext(CreateProperty(), new PipelineSettingsBuilder().WithAddNullChecks().Build(), CultureInfo.InvariantCulture, typeof(string).FullName!, typeof(List<>).WithoutGenerics());
            var evaluator = Fixture.Freeze<IFunctionParseResultEvaluator>();
            var parser = Fixture.Freeze<IExpressionParser>();
            var sut = CreateSut();

            // Act
            var result = sut.Parse(functionParseResult, context, evaluator, parser);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value.Should().Be("is null");
        }

        [Fact]
        public void Returns_Invalid_On_Unsupported_Context_Type()
        {
            // Arrange
            InitializeParser();
            var functionParseResult = new FunctionParseResultBuilder()
                .WithFunctionName("NullCheck")
                .WithFormatProvider(CultureInfo.InvariantCulture)
                .Build();
            var context = this;
            var evaluator = Fixture.Freeze<IFunctionParseResultEvaluator>();
            var parser = Fixture.Freeze<IExpressionParser>();
            var sut = CreateSut();

            // Act
            var result = sut.Parse(functionParseResult, context, evaluator, parser);

            // Assert
            result.Status.Should().Be(ResultStatus.Invalid);
            result.ErrorMessage.Should().Be("NullCheck function does not support type ClassFramework.Pipelines.Tests.Functions.NullCheckFunctionTests+Parse, only ContextBase is supported");
        }

        [Fact]
        public void Returns_Invalid_On_Null_Context()
        {
            // Arrange
            InitializeParser();
            var functionParseResult = new FunctionParseResultBuilder()
                .WithFunctionName("NullCheck")
                .WithFormatProvider(CultureInfo.InvariantCulture)
                .Build();
            object? context = null;
            var evaluator = Fixture.Freeze<IFunctionParseResultEvaluator>();
            var parser = Fixture.Freeze<IExpressionParser>();
            var sut = CreateSut();

            // Act
            var result = sut.Parse(functionParseResult, context, evaluator, parser);

            // Assert
            result.Status.Should().Be(ResultStatus.Invalid);
            result.ErrorMessage.Should().Be("NullCheck function does not support type null, only ContextBase is supported");
        }

        [Fact]
        public void Returns_Continue_On_Wrong_FunctionName()
        {
            // Arrange
            InitializeParser();
            var functionParseResult = new FunctionParseResultBuilder()
                .WithFunctionName("WrongFunctionName")
                .WithFormatProvider(CultureInfo.InvariantCulture)
                .Build();
            object? context = null;
            var evaluator = Fixture.Freeze<IFunctionParseResultEvaluator>();
            var parser = Fixture.Freeze<IExpressionParser>();
            var sut = CreateSut();

            // Act
            var result = sut.Parse(functionParseResult, context, evaluator, parser);

            // Assert
            result.Status.Should().Be(ResultStatus.Continue);
        }
    }
}

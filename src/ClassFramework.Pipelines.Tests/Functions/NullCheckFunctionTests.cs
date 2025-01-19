namespace ClassFramework.Pipelines.Tests.Functions;

public class NullCheckFunctionTests : TestBase<NullCheckFunction>
{
    public class Evaluate : NullCheckFunctionTests
    {
        [Fact]
        public void Returns_Success_On_Context_Of_Type_ContextBase()
        {
            // Arrange
            InitializeParser();
            var functionCall = new FunctionCallBuilder()
                .WithName("NullCheck")
                .Build();
            var context = new PropertyContext(CreateProperty(), new PipelineSettingsBuilder().WithAddNullChecks().Build(), CultureInfo.InvariantCulture, typeof(string).FullName!, typeof(List<>).WithoutGenerics());
            var evaluator = Fixture.Freeze<IFunctionEvaluator>();
            var parser = Fixture.Freeze<IExpressionEvaluator>();
            var sut = CreateSut();
            var functionCallContext = new FunctionCallContext(functionCall, evaluator, parser, CultureInfo.InvariantCulture, context);

            // Act
            var result = sut.Evaluate(functionCallContext);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value.Should().Be("is null");
        }

        [Fact]
        public void Returns_Invalid_On_Unsupported_Context_Type()
        {
            // Arrange
            InitializeParser();
            var functionCall = new FunctionCallBuilder()
                .WithName("NullCheck")
                .Build();
            var context = this;
            var evaluator = Fixture.Freeze<IFunctionEvaluator>();
            var parser = Fixture.Freeze<IExpressionEvaluator>();
            var sut = CreateSut();
            var functionCallContext = new FunctionCallContext(functionCall, evaluator, parser, CultureInfo.InvariantCulture, context);

            // Act
            var result = sut.Evaluate(functionCallContext);

            // Assert
            result.Status.Should().Be(ResultStatus.Invalid);
            result.ErrorMessage.Should().Be("NullCheck function does not support type ClassFramework.Pipelines.Tests.Functions.NullCheckFunctionTests+Evaluate, only ContextBase is supported");
        }

        [Fact]
        public void Returns_Invalid_On_Null_Context()
        {
            // Arrange
            InitializeParser();
            var functionCall = new FunctionCallBuilder()
                .WithName("NullCheck")
                .Build();
            object? context = null;
            var evaluator = Fixture.Freeze<IFunctionEvaluator>();
            var parser = Fixture.Freeze<IExpressionEvaluator>();
            var sut = CreateSut();
            var functionCallContext = new FunctionCallContext(functionCall, evaluator, parser, CultureInfo.InvariantCulture, context);

            // Act
            var result = sut.Evaluate(functionCallContext);

            // Assert
            result.Status.Should().Be(ResultStatus.Invalid);
            result.ErrorMessage.Should().Be("NullCheck function does not support type null, only ContextBase is supported");
        }

        [Fact]
        public void Returns_Invalid_On_Wrong_FunctionName()
        {
            // Arrange
            InitializeParser();
            var functionCall = new FunctionCallBuilder()
                .WithName("WrongFunctionName")
                .Build();
            object? context = null;
            var evaluator = Fixture.Freeze<IFunctionEvaluator>();
            var parser = Fixture.Freeze<IExpressionEvaluator>();
            var sut = CreateSut();
            var functionCallContext = new FunctionCallContext(functionCall, evaluator, parser, CultureInfo.InvariantCulture, context);

            // Act
            var result = sut.Evaluate(functionCallContext);

            // Assert
            result.Status.Should().Be(ResultStatus.Invalid);
        }
    }
}

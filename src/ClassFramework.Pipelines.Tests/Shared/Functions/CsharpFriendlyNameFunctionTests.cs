namespace ClassFramework.Pipelines.Tests.Shared.Functions;

public class CsharpFriendlyNameFunctionTests : TestBase<CsharpFriendlyNameFunction>
{
    public class Parse : CsharpFriendlyNameFunctionTests
    {
        [Fact]
        public void Returns_Continue_When_FunctionName_Is_Invalid()
        {
            // Arrange
            InitializeParser();
            var functionParseResult = new FunctionParseResultBuilder()
                .WithFunctionName("Invalid")
                .WithFormatProvider(CultureInfo.InvariantCulture)
                .Build();
            object? context = default;
            var evaluator = Fixture.Freeze<IFunctionParseResultEvaluator>();
            var parser = Fixture.Freeze<IExpressionParser>();
            var sut = CreateSut();

            // Act
            var result = sut.Parse(functionParseResult, context, evaluator, parser);

            // Assert
            result.Status.Should().Be(ResultStatus.Continue);
        }

        [Fact]
        public void Returns_Invalid_When_No_Arguments_Are_Provided()
        {
            // Arrange
            InitializeParser();
            var functionParseResult = new FunctionParseResultBuilder()
                .WithFunctionName("CsharpFriendlyName")
                .WithFormatProvider(CultureInfo.InvariantCulture)
                .Build();
            object? context = default;
            var evaluator = Fixture.Freeze<IFunctionParseResultEvaluator>();
            var parser = Fixture.Freeze<IExpressionParser>();
            var sut = CreateSut();

            // Act
            var result = sut.Parse(functionParseResult, context, evaluator, parser);

            // Assert
            result.Status.Should().Be(ResultStatus.Invalid);
            result.ErrorMessage.Should().Be("CsharpFriendlyName function requires one argument");
        }

        [Fact]
        public void Returns_InnerResult_When_Argument_ValueResult_Is_Not_Successful()
        {
            // Arrange
            InitializeParser();
            var functionParseResult = new FunctionParseResultBuilder()
                .WithFunctionName("CsharpFriendlyName")
                .WithFormatProvider(CultureInfo.InvariantCulture)
                .AddArguments(new FunctionArgumentBuilder().WithFunction(new FunctionParseResultBuilder().WithFunctionName("Error")))
                .Build();
            object? context = default;
            var evaluator = Fixture.Freeze<IFunctionParseResultEvaluator>();
            evaluator
                .Evaluate(Arg.Any<FunctionParseResult>(), Arg.Any<IExpressionParser>(), Arg.Any<object?>())
                .Returns(Result.Error<object?>("Kaboom"));
            var parser = Fixture.Freeze<IExpressionParser>();
            var sut = CreateSut();

            // Act
            var result = sut.Parse(functionParseResult, context, evaluator, parser);

            // Assert
            result.Status.Should().Be(ResultStatus.Error);
            result.ErrorMessage.Should().Be("Kaboom");
        }

        [Fact]
        public void Returns_Invalid_When_Argument_ValueResult_Is_Not_Of_Type_String()
        {
            // Arrange
            InitializeParser();
            var functionParseResult = new FunctionParseResultBuilder()
                .WithFunctionName("CsharpFriendlyName")
                .WithFormatProvider(CultureInfo.InvariantCulture)
                .AddArguments(new FunctionArgumentBuilder().WithFunction(new FunctionParseResultBuilder().WithFunctionName("Error")))
                .Build();
            object? context = default;
            var evaluator = Fixture.Freeze<IFunctionParseResultEvaluator>();
            evaluator
                .Evaluate(Arg.Any<FunctionParseResult>(), Arg.Any<IExpressionParser>(), Arg.Any<object?>())
                .Returns(Result.Success<object?>(12345));
            var parser = Fixture.Freeze<IExpressionParser>();
            var sut = CreateSut();

            // Act
            var result = sut.Parse(functionParseResult, context, evaluator, parser);

            // Assert
            result.Status.Should().Be(ResultStatus.Invalid);
            result.ErrorMessage.Should().Be("CsharpFriendlyName does not support type System.Int32, only string is supported");
        }

        [Fact]
        public void Returns_Invalid_When_Argument_ValueResult_Is_Null()
        {
            // Arrange
            InitializeParser();
            var functionParseResult = new FunctionParseResultBuilder()
                .WithFunctionName("CsharpFriendlyName")
                .WithFormatProvider(CultureInfo.InvariantCulture)
                .AddArguments(new FunctionArgumentBuilder().WithFunction(new FunctionParseResultBuilder().WithFunctionName("Error")))
                .Build();
            object? context = default;
            var evaluator = Fixture.Freeze<IFunctionParseResultEvaluator>();
            evaluator
                .Evaluate(Arg.Any<FunctionParseResult>(), Arg.Any<IExpressionParser>(), Arg.Any<object?>())
                .Returns(Result.Success<object?>(null));
            var parser = Fixture.Freeze<IExpressionParser>();
            var sut = CreateSut();

            // Act
            var result = sut.Parse(functionParseResult, context, evaluator, parser);

            // Assert
            result.Status.Should().Be(ResultStatus.Invalid);
            result.ErrorMessage.Should().Be("CsharpFriendlyName requires argument of type string, but the value was null");
        }

        [Fact]
        public void Returns_Success_When_Argument_ValueResult_Is_Of_Type_String()
        {
            // Arrange
            InitializeParser();
            var functionParseResult = new FunctionParseResultBuilder()
                .WithFunctionName("CsharpFriendlyName")
                .WithFormatProvider(CultureInfo.InvariantCulture)
                .AddArguments(new FunctionArgumentBuilder().WithFunction(new FunctionParseResultBuilder().WithFunctionName("Error")))
                .Build();
            object? context = default;
            var evaluator = Fixture.Freeze<IFunctionParseResultEvaluator>();
            evaluator
                .Evaluate(Arg.Any<FunctionParseResult>(), Arg.Any<IExpressionParser>(), Arg.Any<object?>())
                .Returns(Result.Success<object?>("delegate"));
            var parser = Fixture.Freeze<IExpressionParser>();
            var sut = CreateSut();

            // Act
            var result = sut.Parse(functionParseResult, context, evaluator, parser);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value.Should().Be("@delegate");
        }
    }
}

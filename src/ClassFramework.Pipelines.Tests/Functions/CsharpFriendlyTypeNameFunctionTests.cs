namespace ClassFramework.Pipelines.Tests.Functions;

public class CsharpFriendlyTypeNameFunctionTests : TestBase<CsharpFriendlyTypeNameFunction>
{
    public class Evaluate : CsharpFriendlyTypeNameFunctionTests
    {
        [Fact]
        public void Returns_Invalid_When_FunctionName_Is_Invalid()
        {
            // Arrange
            InitializeParser();
            var functionCall = new FunctionCallBuilder()
                .WithName("Invalid")
                .Build();
            object? context = default;
            var evaluator = Fixture.Freeze<IFunctionEvaluator>();
            var parser = Fixture.Freeze<IExpressionEvaluator>();
            var sut = CreateSut();
            var functionCallContext = new FunctionCallContext(functionCall, evaluator, parser, CultureInfo.InvariantCulture, context);

            // Act
            var result = sut.Evaluate(functionCallContext);

            // Assert
            result.Status.Should().Be(ResultStatus.Invalid);
        }

        [Fact]
        public void Returns_Invalid_When_No_Arguments_Are_Provided()
        {
            // Arrange
            InitializeParser();
            var functionCall = new FunctionCallBuilder()
                .WithName("CsharpFriendlyTypeName")
                .Build();
            object? context = default;
            var evaluator = Fixture.Freeze<IFunctionEvaluator>();
            var parser = Fixture.Freeze<IExpressionEvaluator>();
            var sut = CreateSut();
            var functionCallContext = new FunctionCallContext(functionCall, evaluator, parser, CultureInfo.InvariantCulture, context);

            // Act
            var result = sut.Evaluate(functionCallContext);

            // Assert
            result.Status.Should().Be(ResultStatus.Invalid);
            result.ErrorMessage.Should().Be("CsharpFriendlyTypeName function requires one argument");
        }

        [Fact]
        public void Returns_InnerResult_When_Argument_ValueResult_Is_Not_Successful()
        {
            // Arrange
            InitializeParser();
            var functionCall = new FunctionCallBuilder()
                .WithName("CsharpFriendlyTypeName")
                .AddArguments(new FunctionArgumentBuilder().WithFunction(new FunctionCallBuilder().WithName("Error")))
                .Build();
            object? context = default;
            var evaluator = Fixture.Freeze<IFunctionEvaluator>();
            evaluator
                .Evaluate(Arg.Any<FunctionCall>(), Arg.Any<IExpressionEvaluator>(), Arg.Any<IFormatProvider>(), Arg.Any<object?>())
                .Returns(Result.Error<object?>("Kaboom"));
            var parser = Fixture.Freeze<IExpressionEvaluator>();
            var sut = CreateSut();
            var functionCallContext = new FunctionCallContext(functionCall, evaluator, parser, CultureInfo.InvariantCulture, context);

            // Act
            var result = sut.Evaluate(functionCallContext);

            // Assert
            result.Status.Should().Be(ResultStatus.Error);
            result.ErrorMessage.Should().Be("Kaboom");
        }

        [Fact]
        public void Returns_Invalid_When_Argument_ValueResult_Is_Not_Of_Type_String()
        {
            // Arrange
            InitializeParser();
            var functionCall = new FunctionCallBuilder()
                .WithName("CsharpFriendlyTypeName")
                .AddArguments(new FunctionArgumentBuilder().WithFunction(new FunctionCallBuilder().WithName("Integer")))
                .Build();
            object? context = default;
            var evaluator = Fixture.Freeze<IFunctionEvaluator>();
            evaluator
                .Evaluate(Arg.Any<FunctionCall>(), Arg.Any<IExpressionEvaluator>(), Arg.Any<IFormatProvider>(), Arg.Any<object?>())
                .Returns(Result.Success<object?>(12345));
            var parser = Fixture.Freeze<IExpressionEvaluator>();
            var sut = CreateSut();
            var functionCallContext = new FunctionCallContext(functionCall, evaluator, parser, CultureInfo.InvariantCulture, context);

            // Act
            var result = sut.Evaluate(functionCallContext);

            // Assert
            result.Status.Should().Be(ResultStatus.Invalid);
            result.ErrorMessage.Should().Be("CsharpFriendlyTypeName function does not support type System.Int32, only string is supported");
        }

        [Fact]
        public void Returns_Invalid_When_Argument_ValueResult_Is_Null()
        {
            // Arrange
            InitializeParser();
            var functionCall = new FunctionCallBuilder()
                .WithName("CsharpFriendlyTypeName")
                .AddArguments(new FunctionArgumentBuilder().WithFunction(new FunctionCallBuilder().WithName("Null")))
                .Build();
            object? context = default;
            var evaluator = Fixture.Freeze<IFunctionEvaluator>();
            evaluator
                .Evaluate(Arg.Any<FunctionCall>(), Arg.Any<IExpressionEvaluator>(), Arg.Any<IFormatProvider>(), Arg.Any<object?>())
                .Returns(Result.Success<object?>(null));
            var parser = Fixture.Freeze<IExpressionEvaluator>();
            var sut = CreateSut();
            var functionCallContext = new FunctionCallContext(functionCall, evaluator, parser, CultureInfo.InvariantCulture, context);

            // Act
            var result = sut.Evaluate(functionCallContext);

            // Assert
            result.Status.Should().Be(ResultStatus.Invalid);
            result.ErrorMessage.Should().Be("CsharpFriendlyTypeName function requires argument of type string, but the value was null");
        }

        [Fact]
        public void Returns_Success_When_Argument_ValueResult_Is_Of_Type_String()
        {
            // Arrange
            InitializeParser();
            var functionCall = new FunctionCallBuilder()
                .WithName("CsharpFriendlyTypeName")
                .AddArguments(new FunctionArgumentBuilder().WithFunction(new FunctionCallBuilder().WithName("MyFunction")))
                .Build();
            object? context = default;
            var evaluator = Fixture.Freeze<IFunctionEvaluator>();
            evaluator
                .Evaluate(Arg.Any<FunctionCall>(), Arg.Any<IExpressionEvaluator>(), Arg.Any<IFormatProvider>(), Arg.Any<object?>())
                .Returns(Result.Success<object?>(typeof(string).FullName));
            var parser = Fixture.Freeze<IExpressionEvaluator>();
            var sut = CreateSut();
            var functionCallContext = new FunctionCallContext(functionCall, evaluator, parser, CultureInfo.InvariantCulture, context);

            // Act
            var result = sut.Evaluate(functionCallContext);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value.Should().Be("string");
        }
    }
}

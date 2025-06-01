namespace ClassFramework.Pipelines.Tests.Functions;

public class ClassNameFunctionTests : TestBase<ClassNameFunction>
{
    public class Evaluate : ClassNameFunctionTests
    {
        [Fact]
        public async Task Returns_Invalid_When_FunctionName_Is_Invalid()
        {
            // Arrange
            await InitializeExpressionEvaluator();
            var functionCall = new FunctionCallBuilder()
                .WithName("Invalid")
                .Build();
            object? context = default;
            var evaluator = Fixture.Freeze<IExpressionEvaluator>();
            var sut = CreateSut();
            var functionCallContext = new FunctionCallContext(functionCall, new ExpressionEvaluatorContext("Dummy", new ExpressionEvaluatorSettingsBuilder(), evaluator, new Dictionary<string, Task<Result<object?>>> { { "context", Task.FromResult(Result.Success(context)) } }));

            // Act
            var result = await sut.EvaluateAsync(functionCallContext, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Invalid);
        }

        [Fact]
        public async Task Returns_Invalid_When_No_Arguments_Are_Provided()
        {
            // Arrange
            await InitializeExpressionEvaluator();
            var functionCall = new FunctionCallBuilder()
                .WithName("ClassName")
                .Build();
            object? context = default;
            var evaluator = Fixture.Freeze<IExpressionEvaluator>();
            var sut = CreateSut();
            var functionCallContext = new FunctionCallContext(functionCall, new ExpressionEvaluatorContext("Dummy", new ExpressionEvaluatorSettingsBuilder(), evaluator, new Dictionary<string, Task<Result<object?>>> { { "context", Task.FromResult(Result.Success(context)) } }));

            // Act
            var result = await sut.EvaluateAsync(functionCallContext, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Invalid);
            result.ErrorMessage.ShouldBe("Missing argument: Expression");
        }

        [Fact]
        public async Task Returns_InnerResult_When_Argument_ValueResult_Is_Not_Successful()
        {
            // Arrange
            await InitializeExpressionEvaluator();
            var functionCall = new FunctionCallBuilder()
                .WithName("ClassName")
                .AddArguments("Error()")
                .Build();
            object? context = default;
            var evaluator = Fixture.Freeze<IExpressionEvaluator>();
            evaluator
                .EvaluateAsync(Arg.Any<ExpressionEvaluatorContext>(), Arg.Any<CancellationToken>())
                .Returns(Result.Error<object?>("Kaboom"));
            var sut = CreateSut();
            var functionCallContext = new FunctionCallContext(functionCall, new ExpressionEvaluatorContext("Dummy", new ExpressionEvaluatorSettingsBuilder(), evaluator, new Dictionary<string, Task<Result<object?>>> { { "context", Task.FromResult(Result.Success(context)) } }));

            // Act
            var result = await sut.EvaluateAsync(functionCallContext, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Error);
            result.ErrorMessage.ShouldBe("Kaboom");
        }

        [Fact]
        public async Task Returns_Invalid_When_Argument_ValueResult_Is_Not_Of_Type_String()
        {
            // Arrange
            await InitializeExpressionEvaluator();
            var functionCall = new FunctionCallBuilder()
                .WithName("ClassName")
                .AddArguments("Numeric()")
                .Build();
            object? context = default;
            var evaluator = Fixture.Freeze<IExpressionEvaluator>();
            evaluator
                .EvaluateAsync(Arg.Any<ExpressionEvaluatorContext>(), Arg.Any<CancellationToken>())
                .Returns(Result.Success<object?>(12345));
            var sut = CreateSut();
            var functionCallContext = new FunctionCallContext(functionCall, new ExpressionEvaluatorContext("Dummy", new ExpressionEvaluatorSettingsBuilder(), evaluator, new Dictionary<string, Task<Result<object?>>> { { "context", Task.FromResult(Result.Success(context)) } }));

            // Act
            var result = await sut.EvaluateAsync(functionCallContext, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Invalid);
            result.ErrorMessage.ShouldBe("Expression is not of type string");
        }

        [Fact]
        public async Task Returns_Invalid_When_Argument_ValueResult_Is_Null()
        {
            // Arrange
            await InitializeExpressionEvaluator();
            var functionCall = new FunctionCallBuilder()
                .WithName("ClassName")
                .AddArguments("Null()")
                .Build();
            object? context = default;
            var evaluator = Fixture.Freeze<IExpressionEvaluator>();
            evaluator
                .EvaluateAsync(Arg.Any<ExpressionEvaluatorContext>(), Arg.Any<CancellationToken>())
                .Returns(Result.Success<object?>(null));
            var sut = CreateSut();
            var functionCallContext = new FunctionCallContext(functionCall, new ExpressionEvaluatorContext("Dummy", new ExpressionEvaluatorSettingsBuilder(), evaluator, new Dictionary<string, Task<Result<object?>>> { { "context", Task.FromResult(Result.Success(context)) } }));

            // Act
            var result = await sut.EvaluateAsync(functionCallContext, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Invalid);
            result.ErrorMessage.ShouldBe("Expression is not of type string");
        }

        [Fact]
        public async Task Returns_Success_When_Argument_ValueResult_Is_Of_Type_String()
        {
            // Arrange
            await InitializeExpressionEvaluator();
            var functionCall = new FunctionCallBuilder()
                .WithName("ClassName")
                .AddArguments("MyStringFunction()")
                .Build();
            object? context = default;
            var evaluator = Fixture.Freeze<IExpressionEvaluator>();
            evaluator
                .EvaluateAsync(Arg.Any<ExpressionEvaluatorContext>(), Arg.Any<CancellationToken>())
                .Returns(Result.Success<object?>("MyNamespace.MyClass"));
            var sut = CreateSut();
            var functionCallContext = new FunctionCallContext(functionCall, new ExpressionEvaluatorContext("Dummy", new ExpressionEvaluatorSettingsBuilder(), evaluator, new Dictionary<string, Task<Result<object?>>> { { "context", Task.FromResult(Result.Success(context)) } }));

            // Act
            var result = await sut.EvaluateAsync(functionCallContext, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            result.Value.ShouldBe("MyClass");
        }
    }
}

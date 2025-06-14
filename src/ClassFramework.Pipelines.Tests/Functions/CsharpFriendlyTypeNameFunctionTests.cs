namespace ClassFramework.Pipelines.Tests.Functions;

public class CsharpFriendlyTypeNameFunctionTests : TestBase<CsharpFriendlyTypeNameFunction>
{
    public class Evaluate : CsharpFriendlyTypeNameFunctionTests
    {
        [Fact]
        public async Task Returns_Invalid_When_FunctionName_Is_Invalid()
        {
            // Arrange
            await InitializeExpressionEvaluatorAsync();
            var functionCall = new FunctionCallBuilder()
                .WithName("Invalid")
                .WithMemberType(MemberType.Function)
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
            await InitializeExpressionEvaluatorAsync();
            var functionCall = new FunctionCallBuilder()
                .WithName("CsharpFriendlyTypeName")
                .WithMemberType(MemberType.Function)
                .Build();
            object? context = default;
            var evaluator = Fixture.Freeze<IExpressionEvaluator>();
            var sut = CreateSut();
            var functionCallContext = new FunctionCallContext(functionCall, new ExpressionEvaluatorContext("Dummy", new ExpressionEvaluatorSettingsBuilder(), evaluator, new Dictionary<string, Task<Result<object?>>> { { "context", Task.FromResult(Result.Success(context)) } }));

            // Act
            var result = await sut.EvaluateAsync(functionCallContext, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Invalid);
            result.ErrorMessage.ShouldBe("CsharpFriendlyTypeName function failed, see inner results for details");
            result.InnerResults.Count.ShouldBe(1);
            result.InnerResults.First().ErrorMessage.ShouldBe("Missing argument: Expression");
        }

        [Fact]
        public async Task Returns_InnerResult_When_Argument_ValueResult_Is_Not_Successful()
        {
            // Arrange
            await InitializeExpressionEvaluatorAsync();
            var functionCall = new FunctionCallBuilder()
                .WithName("CsharpFriendlyTypeName")
                .WithMemberType(MemberType.Function)
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
            result.ErrorMessage.ShouldBe("CsharpFriendlyTypeName function failed, see inner results for details");
            result.InnerResults.Count.ShouldBe(1);
            result.InnerResults.First().ErrorMessage.ShouldBe("Kaboom");
        }

        [Fact]
        public async Task Returns_Invalid_When_Argument_ValueResult_Is_Not_Of_Type_String()
        {
            // Arrange
            await InitializeExpressionEvaluatorAsync();
            var functionCall = new FunctionCallBuilder()
                .WithName("CsharpFriendlyTypeName")
                .WithMemberType(MemberType.Function)
                .AddArguments("Integer()")
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
            result.ErrorMessage.ShouldBe("CsharpFriendlyTypeName function failed, see inner results for details");
            result.InnerResults.Count.ShouldBe(1);
            result.InnerResults.First().ErrorMessage.ShouldBe("Could not cast System.Int32 to System.String");
        }

        [Fact]
        public async Task Returns_Invalid_When_Argument_ValueResult_Is_Null()
        {
            // Arrange
            await InitializeExpressionEvaluatorAsync();
            var functionCall = new FunctionCallBuilder()
                .WithName("CsharpFriendlyTypeName")
                .WithMemberType(MemberType.Function)
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
            result.ErrorMessage.ShouldBe("CsharpFriendlyTypeName function failed, see inner results for details");
            result.InnerResults.Count.ShouldBe(1);
            result.InnerResults.First().ErrorMessage.ShouldBe("Could not cast  to System.String");
        }

        [Fact]
        public async Task Returns_Success_When_Argument_ValueResult_Is_Of_Type_String()
        {
            // Arrange
            await InitializeExpressionEvaluatorAsync();
            var functionCall = new FunctionCallBuilder()
                .WithName("CsharpFriendlyTypeName")
                .WithMemberType(MemberType.Function)
                .AddArguments("MyFunction()")
                .Build();
            object? context = default;
            var evaluator = Fixture.Freeze<IExpressionEvaluator>();
            evaluator
                .EvaluateAsync(Arg.Any<ExpressionEvaluatorContext>(), Arg.Any<CancellationToken>())
                .Returns(Result.Success<object?>(typeof(string).FullName));
            var sut = CreateSut();
            var functionCallContext = new FunctionCallContext(functionCall, new ExpressionEvaluatorContext("Dummy", new ExpressionEvaluatorSettingsBuilder(), evaluator, new Dictionary<string, Task<Result<object?>>> { { "context", Task.FromResult(Result.Success(context)) } }));

            // Act
            var result = await sut.EvaluateAsync(functionCallContext, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            result.Value.ShouldBe("string");
        }
    }
}

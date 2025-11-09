
namespace ClassFramework.Pipelines.Tests.Functions;

public class NullCheckFunctionTests : TestBase<NullCheckFunction>
{
    public class Evaluate : NullCheckFunctionTests
    {
        [Fact]
        public async Task Returns_Success_On_Context_Of_Type_ContextBase()
        {
            // Arrange
            await InitializeExpressionEvaluatorAsync();
            var functionCall = new FunctionCallBuilder()
                .WithName("NullCheck")
                .WithMemberType(MemberType.Function)
                .Build();
            var settings = new PipelineSettingsBuilder().Build();
            var formatProvider = Fixture.Freeze<IFormatProvider>();
            var command = new TestCommand(settings, formatProvider); var evaluator = Fixture.Freeze<IExpressionEvaluator>();
            var sut = CreateSut();
            var functionCallContext = new FunctionCallContext(functionCall, new ExpressionEvaluatorContext("Dummy", new ExpressionEvaluatorSettingsBuilder(), evaluator, new Dictionary<string, Func<Task<Result<object?>>>> { { "context", () => Task.FromResult(Result.Success<object?>(command)) } }));

            // Act
            var result = await sut.EvaluateAsync(functionCallContext, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            result.Value.ShouldBe("is null");
        }

        [Fact]
        public async Task Returns_Invalid_On_Unsupported_Context_Type()
        {
            // Arrange
            await InitializeExpressionEvaluatorAsync();
            var functionCall = new FunctionCallBuilder()
                .WithName("NullCheck")
                .WithMemberType(MemberType.Function)
                .Build();
            var context = this;
            var evaluator = Fixture.Freeze<IExpressionEvaluator>();
            var sut = CreateSut();
            var functionCallContext = new FunctionCallContext(functionCall, new ExpressionEvaluatorContext("Dummy", new ExpressionEvaluatorSettingsBuilder(), evaluator, new Dictionary<string, Func<Task<Result<object?>>>> { { "context", () => Task.FromResult(Result.Success<object?>(context)) } }));

            // Act
            var result = await sut.EvaluateAsync(functionCallContext, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Invalid);
            result.ErrorMessage.ShouldBe("NullCheck function does not support type ClassFramework.Pipelines.Tests.Functions.NullCheckFunctionTests+Evaluate, only ContextBase is supported");
        }

        [Fact]
        public async Task Returns_Invalid_On_Null_Context()
        {
            // Arrange
            await InitializeExpressionEvaluatorAsync();
            var functionCall = new FunctionCallBuilder()
                .WithName("NullCheck")
                .WithMemberType(MemberType.Function)
                .Build();
            object? context = null;
            var evaluator = Fixture.Freeze<IExpressionEvaluator>();
            var sut = CreateSut();
            var functionCallContext = new FunctionCallContext(functionCall, new ExpressionEvaluatorContext("Dummy", new ExpressionEvaluatorSettingsBuilder(), evaluator, new Dictionary<string, Func<Task<Result<object?>>>> { { "context", () => Task.FromResult(Result.Success(context)) } }));

            // Act
            var result = await sut.EvaluateAsync(functionCallContext, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Invalid);
            result.ErrorMessage.ShouldBe("NullCheck function does not support type null, only ContextBase is supported");
        }

        [Fact]
        public async Task Returns_Invalid_On_Wrong_FunctionName()
        {
            // Arrange
            await InitializeExpressionEvaluatorAsync();
            var functionCall = new FunctionCallBuilder()
                .WithName("WrongFunctionName")
                .WithMemberType(MemberType.Function)
                .Build();
            object? context = null;
            var evaluator = Fixture.Freeze<IExpressionEvaluator>();
            var sut = CreateSut();
            var functionCallContext = new FunctionCallContext(functionCall, new ExpressionEvaluatorContext("Dummy", new ExpressionEvaluatorSettingsBuilder(), evaluator, new Dictionary<string, Func<Task<Result<object?>>>> { { "context", () => Task.FromResult(Result.Success(context)) } }));

            // Act
            var result = await sut.EvaluateAsync(functionCallContext, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Invalid);
        }

        private sealed class TestCommand(PipelineSettings settings, IFormatProvider formatProvider) : CommandBase<string>(string.Empty, settings, formatProvider, CancellationToken.None)
        {
            protected override string NewCollectionTypeName => string.Empty;
            public override Task<Result<TypeBaseBuilder>> ExecuteCommandAsync<TContext>(ICommandService commandService, TContext command, CancellationToken token) => throw new NotImplementedException();
            public override bool SourceModelHasNoProperties() => throw new NotImplementedException();
        }
    }
}

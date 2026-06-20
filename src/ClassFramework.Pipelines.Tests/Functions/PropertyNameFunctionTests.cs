namespace ClassFramework.Pipelines.Tests.Functions;

public class PropertyNameFunctionTests : TestBase<PropertyNameFunction>
{
    public class Evaluate : PropertyNameFunctionTests
    {
        [Fact]
        public async Task Returns_Plain_Property_Name_When_AddProperties_Is_True()
        {
            // Arrange
            await InitializeExpressionEvaluatorAsync();
            var functionCall = new FunctionCallBuilder()
                .WithName("PropertyName")
                .WithMemberType(MemberType.Function)
                .Build();
            var settings = new PipelineSettingsBuilder().WithAddProperties(true).Build();
            var formatProvider = Fixture.Freeze<IFormatProvider>();
            var command = new TestCommand(settings, formatProvider); var evaluator = Fixture.Freeze<IExpressionEvaluator>();
            var sut = CreateSut();
            var functionCallContext = new FunctionCallContext(functionCall, new ExpressionEvaluatorContext("Dummy", new ExpressionEvaluatorSettingsBuilder(), evaluator, new Dictionary<string, Func<Task<Result<object?>>>> { { "context", () => Task.FromResult(Result.Success<object?>(command)) } }));

            // Act
            var result = await sut.EvaluateAsync(functionCallContext, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            result.Value.ShouldBe("{property.Name}");
        }

        [Fact]
        public async Task Returns_BackingField_Name_When_AddProperties_Is_False()
        {
            // Arrange
            await InitializeExpressionEvaluatorAsync();
            var functionCall = new FunctionCallBuilder()
                .WithName("PropertyName")
                .WithMemberType(MemberType.Function)
                .Build();
            var settings = new PipelineSettingsBuilder().WithAddProperties(false).Build();
            var formatProvider = Fixture.Freeze<IFormatProvider>();
            var command = new TestCommand(settings, formatProvider); var evaluator = Fixture.Freeze<IExpressionEvaluator>();
            var sut = CreateSut();
            var functionCallContext = new FunctionCallContext(functionCall, new ExpressionEvaluatorContext("Dummy", new ExpressionEvaluatorSettingsBuilder(), evaluator, new Dictionary<string, Func<Task<Result<object?>>>> { { "context", () => Task.FromResult(Result.Success<object?>(command)) } }));

            // Act
            var result = await sut.EvaluateAsync(functionCallContext, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            result.Value.ShouldBe("_{property.Name.ToCamelCase()}");
        }

        private sealed class TestCommand(PipelineSettings settings, IFormatProvider formatProvider) : CommandBase<string>(string.Empty, settings, formatProvider)
        {
            protected override string NewCollectionTypeName => string.Empty;
            public override Task<Result<TypeBaseBuilder>> ExecuteCommandAsync<TContext>(ICommandService commandService, TContext command, CancellationToken token) => throw new NotImplementedException();
            public override bool SourceModelHasNoProperties() => throw new NotImplementedException();
        }
    }
}

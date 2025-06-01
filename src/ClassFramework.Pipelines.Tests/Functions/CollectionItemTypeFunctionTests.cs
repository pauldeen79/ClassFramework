namespace ClassFramework.Pipelines.Tests.Functions;

public class CollectionItemTypeFunctionTests : TestBase<CollectionItemTypeFunction>
{

    [Fact]
    public async Task Returns_Success_When_Argument_ValueResult_Is_Of_Type_String()
    {
        // Arrange
        await InitializeExpressionEvaluator();
        var functionCall = new FunctionCallBuilder()
            .WithName("ClassName")
            .WithMemberType(MemberType.Function)
            .AddArguments("MyFunction()")
            .Build();
        object? context = default;
        var evaluator = Fixture.Freeze<IExpressionEvaluator>();
        evaluator
            .EvaluateAsync(Arg.Any<ExpressionEvaluatorContext>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success<object?>("System.Collections.Generic.IEnumerable<MyClass>"));
        var sut = CreateSut();
        var functionCallContext = new FunctionCallContext(functionCall, new ExpressionEvaluatorContext("Dummy", new ExpressionEvaluatorSettingsBuilder(), evaluator, new Dictionary<string, Task<Result<object?>>> { { "context", Task.FromResult(Result.Success(context)) } }));

        // Act
        var result = await sut.EvaluateAsync(functionCallContext, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        result.Value.ShouldBe("MyClass");
    }
}

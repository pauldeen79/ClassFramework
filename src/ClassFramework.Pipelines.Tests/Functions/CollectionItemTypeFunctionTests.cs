namespace ClassFramework.Pipelines.Tests.Functions;

public class CollectionItemTypeFunctionTests : TestBase<CollectionItemTypeFunction>
{

    [Fact]
    public void Returns_Success_When_Argument_ValueResult_Is_Of_Type_String()
    {
        // Arrange
        InitializeParser();
        var functionCall = new FunctionCallBuilder()
            .WithName("ClassName")
            .AddArguments(new FunctionArgumentBuilder().WithFunction(new FunctionCallBuilder().WithName("MyFunction")))
            .Build();
        object? context = default;
        var evaluator = Fixture.Freeze<IFunctionEvaluator>();
        evaluator
            .Evaluate(Arg.Any<FunctionCall>(), Arg.Any<IFormatProvider>(), Arg.Any<object?>())
            .Returns(Result.Success<object?>("System.Collections.Generic.IEnumerable<MyClass>"));
        var parser = Fixture.Freeze<IExpressionEvaluator>();
        var sut = CreateSut();
        var functionCallContext = new FunctionCallContext(functionCall, evaluator, parser, CultureInfo.InvariantCulture, context);

        // Act
        var result = sut.Evaluate(functionCallContext);

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value.Should().Be("MyClass");
    }
}

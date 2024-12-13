namespace ClassFramework.Pipelines.Tests.Shared.Functions;

public class NullCheckFunctionTests : TestBase<NullCheckFunction>
{
    [Fact]
    public void Can_Get_NullCheck_From_ContextBase()
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
    public void Supplying_Unknown_Context_Type_Gives_Invalid_Result()
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
        result.ErrorMessage.Should().Be("NullCheck function does not support type ClassFramework.Pipelines.Tests.Shared.Functions.NullCheckFunctionTests, only ContextBase is supported");
    }

    [Fact]
    public void Supplying_Null_Context_Gives_Invalid_Result()
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
    public void Sopplying_Wrong_FunctionName_Gives_Continue_Result()
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

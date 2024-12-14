namespace ClassFramework.Pipelines.Tests.Functions;

public class GenericArgumentsFunctionTests : TestBase<GenericArgumentsFunction>
{
    public class Parse : GenericArgumentsFunctionTests
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
                .WithFunctionName("GenericArguments")
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
            result.ErrorMessage.Should().Be("GenericArguments function requires one argument");
        }

        [Fact]
        public void Returns_InnerResult_When_Argument_ValueResult_Is_Not_Successful()
        {
            // Arrange
            InitializeParser();
            var functionParseResult = new FunctionParseResultBuilder()
                .WithFunctionName("GenericArguments")
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
        public void Returns_InnerResult_When_Second_Argument_ValueResult_Is_Not_Successful()
        {
            // Arrange
            InitializeParser();
            var functionParseResult = new FunctionParseResultBuilder()
                .WithFunctionName("GenericArguments")
                .WithFormatProvider(CultureInfo.InvariantCulture)
                .AddArguments(new FunctionArgumentBuilder().WithFunction(new FunctionParseResultBuilder().WithFunctionName("Success")))
                .AddArguments(new FunctionArgumentBuilder().WithFunction(new FunctionParseResultBuilder().WithFunctionName("Kaboom")))
                .Build();
            object? context = default;
            var evaluator = Fixture.Freeze<IFunctionParseResultEvaluator>();
            evaluator
                .Evaluate(Arg.Any<FunctionParseResult>(), Arg.Any<IExpressionParser>(), Arg.Any<object?>())
                .Returns(x => x.ArgAt<FunctionParseResult>(0).FunctionName == "Success" ? Result.Success<object?>("Success") : Result.Error<object?>("Kaboom"));
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
                .WithFunctionName("GenericArguments")
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
            result.ErrorMessage.Should().Be("GenericArguments function does not support type System.Int32, only string is supported");
        }

        [Fact]
        public void Returns_Invalid_When_Argument_ValueResult_Is_Null()
        {
            // Arrange
            InitializeParser();
            var functionParseResult = new FunctionParseResultBuilder()
                .WithFunctionName("GenericArguments")
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
            result.ErrorMessage.Should().Be("GenericArguments function requires argument of type string, but the value was null");
        }

        [Fact]
        public void Returns_Invalid_When_Second_Argument_ValueResult_Is_Not_Of_Type_Boolean()
        {
            // Arrange
            InitializeParser();
            var functionParseResult = new FunctionParseResultBuilder()
                .WithFunctionName("GenericArguments")
                .WithFormatProvider(CultureInfo.InvariantCulture)
                .AddArguments(new FunctionArgumentBuilder().WithFunction(new FunctionParseResultBuilder().WithFunctionName("Error")))
                .AddArguments(new LiteralArgumentBuilder().WithValue("string"))
                .Build();
            object? context = default;
            var evaluator = Fixture.Freeze<IFunctionParseResultEvaluator>();
            evaluator
                .Evaluate(Arg.Any<FunctionParseResult>(), Arg.Any<IExpressionParser>(), Arg.Any<object?>())
                .Returns(Result.Success<object?>("Kaboom"));
            var parser = Fixture.Freeze<IExpressionParser>();
            parser
                .Parse(Arg.Any<string>(), Arg.Any<IFormatProvider>(), Arg.Any<object?>())
                .Returns(Result.Success<object?>("string"));
            var sut = CreateSut();

            // Act
            var result = sut.Parse(functionParseResult, context, evaluator, parser);

            // Assert
            result.Status.Should().Be(ResultStatus.Invalid);
            result.ErrorMessage.Should().Be("GenericArguments function second argument (add brackets) should be boolean");
        }

        [Fact]
        public void Returns_Success_When_Argument_ValueResult_Is_Of_Type_String()
        {
            // Arrange
            InitializeParser();
            var functionParseResult = new FunctionParseResultBuilder()
                .WithFunctionName("GenericArguments")
                .WithFormatProvider(CultureInfo.InvariantCulture)
                .AddArguments(new FunctionArgumentBuilder().WithFunction(new FunctionParseResultBuilder().WithFunctionName("Error")))
                .Build();
            object? context = default;
            var evaluator = Fixture.Freeze<IFunctionParseResultEvaluator>();
            evaluator
                .Evaluate(Arg.Any<FunctionParseResult>(), Arg.Any<IExpressionParser>(), Arg.Any<object?>())
                .Returns(Result.Success<object?>("MyGenericType<System.String>"));
            var parser = Fixture.Freeze<IExpressionParser>();
            var sut = CreateSut();

            // Act
            var result = sut.Parse(functionParseResult, context, evaluator, parser);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value.Should().Be(typeof(string).FullName);
        }

        [Fact]
        public void Returns_Success_When_Argument_ValueResult_Is_Of_Type_String_With_AddBrackets_False()
        {
            // Arrange
            InitializeParser();
            var functionParseResult = new FunctionParseResultBuilder()
                .WithFunctionName("GenericArguments")
                .WithFormatProvider(CultureInfo.InvariantCulture)
                .AddArguments(new FunctionArgumentBuilder().WithFunction(new FunctionParseResultBuilder().WithFunctionName("Error")))
                .AddArguments(new LiteralArgumentBuilder().WithValue("false"))
                .Build();
            object? context = default;
            var evaluator = Fixture.Freeze<IFunctionParseResultEvaluator>();
            evaluator
                .Evaluate(Arg.Any<FunctionParseResult>(), Arg.Any<IExpressionParser>(), Arg.Any<object?>())
                .Returns(Result.Success<object?>("MyGenericType<System.String>"));
            var parser = Fixture.Freeze<IExpressionParser>();
            parser
                .Parse(Arg.Any<string>(), Arg.Any<IFormatProvider>(), Arg.Any<object?>())
                .Returns(Result.Success<object?>(false));
            var sut = CreateSut();

            // Act
            var result = sut.Parse(functionParseResult, context, evaluator, parser);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value.Should().Be(typeof(string).FullName);
        }

        [Fact]
        public void Returns_Success_When_Argument_ValueResult_Is_Of_Type_String_With_AddBrackets_True()
        {
            // Arrange
            InitializeParser();
            var functionParseResult = new FunctionParseResultBuilder()
                .WithFunctionName("GenericArguments")
                .WithFormatProvider(CultureInfo.InvariantCulture)
                .AddArguments(new FunctionArgumentBuilder().WithFunction(new FunctionParseResultBuilder().WithFunctionName("Error")))
                .AddArguments(new LiteralArgumentBuilder().WithValue("true"))
                .Build();
            object? context = default;
            var evaluator = Fixture.Freeze<IFunctionParseResultEvaluator>();
            evaluator
                .Evaluate(Arg.Any<FunctionParseResult>(), Arg.Any<IExpressionParser>(), Arg.Any<object?>())
                .Returns(Result.Success<object?>("MyGenericType<System.String>"));
            var parser = Fixture.Freeze<IExpressionParser>();
            parser
                .Parse(Arg.Any<string>(), Arg.Any<IFormatProvider>(), Arg.Any<object?>())
                .Returns(Result.Success<object?>(true));
            var sut = CreateSut();

            // Act
            var result = sut.Parse(functionParseResult, context, evaluator, parser);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value.Should().Be($"<{typeof(string).FullName}>");
        }
    }
}

namespace ClassFramework.Pipelines.Tests.Functions;

public class GenericArgumentsFunctionTests : TestBase<GenericArgumentsFunction>
{
    public class Evaluate : GenericArgumentsFunctionTests
    {
        [Fact]
        public async Task Returns_Invalid_When_FunctionName_Is_Invalid()
        {
            // Arrange
            await InitializeExpressionEvaluator();
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
            await InitializeExpressionEvaluator();
            var functionCall = new FunctionCallBuilder()
                .WithName("GenericArguments")
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
            result.ErrorMessage.ShouldBe("Missing argument: Expression");
        }

        [Fact]
        public async Task Returns_InnerResult_When_Argument_ValueResult_Is_Not_Successful()
        {
            // Arrange
            await InitializeExpressionEvaluator();
            var functionCall = new FunctionCallBuilder()
                .WithName("GenericArguments")
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
            result.ErrorMessage.ShouldBe("Kaboom");
        }

        [Fact]
        public async Task Returns_InnerResult_When_Second_Argument_ValueResult_Is_Not_Successful()
        {
            // Arrange
            await InitializeExpressionEvaluator();
            var functionCall = new FunctionCallBuilder()
                .WithName("GenericArguments")
                .WithMemberType(MemberType.Function)
                .AddArguments("Success()", "Kaboom()")
                .Build();
            object? context = default;
            var evaluator = Fixture.Freeze<IExpressionEvaluator>();
            evaluator
                .EvaluateAsync(Arg.Any<ExpressionEvaluatorContext>(), Arg.Any<CancellationToken>())
                .Returns(x => x.ArgAt<ExpressionEvaluatorContext>(0).Expression == "Success()"
                    ? Result.Success<object?>("Success")
                    : Result.Error<object?>("Kaboom"));
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
                .WithName("GenericArguments")
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
            result.ErrorMessage.ShouldBe("Could not cast System.Int32 to System.String");
        }

        [Fact]
        public async Task Returns_Invalid_When_Argument_ValueResult_Is_Null()
        {
            // Arrange
            await InitializeExpressionEvaluator();
            var functionCall = new FunctionCallBuilder()
                .WithName("GenericArguments")
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
            result.ErrorMessage.ShouldBe("Could not cast  to System.String");
        }

        [Fact]
        public async Task Returns_Invalid_When_Second_Argument_ValueResult_Is_Not_Of_Type_Boolean()
        {
            // Arrange
            await InitializeExpressionEvaluator();
            var functionCall = new FunctionCallBuilder()
                .WithName("GenericArguments")
                .WithMemberType(MemberType.Function)
                .AddArguments("Error()", "string")
                .Build();
            object? context = default;
            var evaluator = Fixture.Freeze<IExpressionEvaluator>();
            evaluator
                .EvaluateAsync(Arg.Any<ExpressionEvaluatorContext>(), Arg.Any<CancellationToken>())
                .Returns(x => x.ArgAt<ExpressionEvaluatorContext>(0).Expression switch
                {
                    "Error()" => Result.Success<object?>("Kaboom"),
                    "string" => Result.Success<object?>("string"),
                    _ => Result.NotSupported<object?>()
                });
            var sut = CreateSut();
            var functionCallContext = new FunctionCallContext(functionCall, new ExpressionEvaluatorContext("Dummy", new ExpressionEvaluatorSettingsBuilder(), evaluator, new Dictionary<string, Task<Result<object?>>> { { "context", Task.FromResult(Result.Success(context)) } }));

            // Act
            var result = await sut.EvaluateAsync(functionCallContext, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Invalid);
            result.ErrorMessage.ShouldBe("GenericArguments function second argument (add brackets) should be boolean");
        }

        [Fact]
        public async Task Returns_Success_When_Argument_ValueResult_Is_Of_Type_String()
        {
            // Arrange
            await InitializeExpressionEvaluator();
            var functionCall = new FunctionCallBuilder()
                .WithName("GenericArguments")
                .WithMemberType(MemberType.Function)
                .AddArguments("MyFunction()")
                .Build();
            object? context = default;
            var evaluator = Fixture.Freeze<IExpressionEvaluator>();
            evaluator
                .EvaluateAsync(Arg.Any<ExpressionEvaluatorContext>(), Arg.Any<CancellationToken>())
                .Returns(Result.Success<object?>("MyGenericType<System.String>"));
            var sut = CreateSut();
            var functionCallContext = new FunctionCallContext(functionCall, new ExpressionEvaluatorContext("Dummy", new ExpressionEvaluatorSettingsBuilder(), evaluator, new Dictionary<string, Task<Result<object?>>> { { "context", Task.FromResult(Result.Success(context)) } }));

            // Act
            var result = await sut.EvaluateAsync(functionCallContext, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            result.Value.ShouldBe(typeof(string).FullName);
        }

        [Fact]
        public async Task Returns_Success_When_Argument_ValueResult_Is_Of_Type_String_With_AddBrackets_False()
        {
            // Arrange
            await InitializeExpressionEvaluator();
            var functionCall = new FunctionCallBuilder()
                .WithName("GenericArguments")
                .WithMemberType(MemberType.Function)
                .AddArguments("MyFunction()", "false")
                .Build();
            object? context = default;
            var evaluator = Fixture.Freeze<IExpressionEvaluator>();
            evaluator
                .EvaluateAsync(Arg.Any<ExpressionEvaluatorContext>(), Arg.Any<CancellationToken>())
                .Returns(x => x.ArgAt<ExpressionEvaluatorContext>(0).Expression switch
                {
                    "MyFunction()" => Result.Success<object?>("MyGenericType<System.String>"),
                    "false" => Result.Success<object?>(false),
                    _ => Result.NotSupported<object?>()
                });
            var sut = CreateSut();
            var functionCallContext = new FunctionCallContext(functionCall, new ExpressionEvaluatorContext("Dummy", new ExpressionEvaluatorSettingsBuilder(), evaluator, new Dictionary<string, Task<Result<object?>>> { { "context", Task.FromResult(Result.Success(context)) } }));

            // Act
            var result = await sut.EvaluateAsync(functionCallContext, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            result.Value.ShouldBe(typeof(string).FullName);
        }

        [Fact]
        public async Task Returns_Success_When_Argument_ValueResult_Is_Of_Type_String_With_AddBrackets_True()
        {
            // Arrange
            await InitializeExpressionEvaluator();
            var functionCall = new FunctionCallBuilder()
                .WithName("GenericArguments")
                .WithMemberType(MemberType.Function)
                .AddArguments("MyFunction()", "true")
                .Build();
            object? context = default;
            var evaluator = Fixture.Freeze<IExpressionEvaluator>();
            evaluator
                .EvaluateAsync(Arg.Any<ExpressionEvaluatorContext>(), Arg.Any<CancellationToken>())
                .Returns(x => x.ArgAt<ExpressionEvaluatorContext>(0).Expression switch
                {
                    "MyFunction()" => Result.Success<object?>("MyGenericType<System.String>"),
                    "true" => Result.Success<object?>(true),
                    _ => Result.NotSupported<object?>()
                });
            var sut = CreateSut();
            var functionCallContext = new FunctionCallContext(functionCall, new ExpressionEvaluatorContext("Dummy", new ExpressionEvaluatorSettingsBuilder(), evaluator, new Dictionary<string, Task<Result<object?>>> { { "context", Task.FromResult(Result.Success(context)) } }));

            // Act
            var result = await sut.EvaluateAsync(functionCallContext, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            result.Value.ShouldBe($"<{typeof(string).FullName}>");
        }
    }
}

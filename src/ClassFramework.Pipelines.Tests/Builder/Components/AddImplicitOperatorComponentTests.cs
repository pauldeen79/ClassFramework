namespace ClassFramework.Pipelines.Tests.Builder.Components;

public class AddImplicitOperatorComponentTests : TestBase<Pipelines.Builder.Components.AddImplicitOperatorComponent>
{
    public class ExecuteAsync : AddImplicitOperatorComponentTests
    {
        [Fact]
        public async Task Throws_On_Null_Command()
        {
            // Arrange
            var sut = CreateSut();
            var response = new ClassBuilder();

            // Act & Assert
            var t = sut.ExecuteAsync(command: null!, response, CommandService, CancellationToken.None);
            (await Should.ThrowAsync<ArgumentNullException>(t))
             .ParamName.ShouldBe("command");
        }

        [Fact]
        public async Task Does_Not_Add_Operator_When_Setting_Is_False()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder().WithAddImplicitOperatorOnBuilder(false);
            var command = new GenerateBuilderCommand(CreateClass(), settings, CultureInfo.InvariantCulture);
            var sut = CreateSut();
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Continue);
            response.Methods.ShouldBeEmpty();
        }

        [Fact]
        public async Task Returns_ErrorResult_From_Parsing_Name()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder().WithAddImplicitOperatorOnBuilder(true);
            var command = new GenerateBuilderCommand(CreateGenericClass(addProperties: false), settings, CultureInfo.InvariantCulture);
            await InitializeExpressionEvaluatorAsync(forceError: true);
            var sut = CreateSut();
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Error);
            result.ErrorMessage.ShouldBe("Kaboom");
        }

        [Fact]
        public async Task Adds_Operator_For_Core_Model_Without_Generics()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder().WithAddImplicitOperatorOnBuilder(true);
            var command = new GenerateBuilderCommand(CreateClass(), settings, CultureInfo.InvariantCulture);
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            response.Methods.Count.ShouldBe(1);
            response.Methods.Single().Name.ShouldBe("SomeNamespace.SomeClass");
            response.Methods.Single().Operator.ShouldBeTrue();
            response.Methods.Single().Parameters.Count.ShouldBe(1);
            response.Methods.Single().Parameters.Single().Name.ShouldBe("builder");
            response.Methods.Single().Parameters.Single().TypeName.ShouldBe("SomeClassBuilder");
            response.Methods.Single().ReturnTypeName.ShouldBe("implicit");
        }

        [Fact]
        public async Task Adds_Operator_For_Core_Model_With_Generics()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder().WithAddImplicitOperatorOnBuilder(true);
            var command = new GenerateBuilderCommand(CreateGenericClass(addProperties: false), settings, CultureInfo.InvariantCulture);
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            response.Methods.Count.ShouldBe(1);
            response.Methods.Single().Name.ShouldBe("MyNamespace.MyClass<T>");
            response.Methods.Single().Operator.ShouldBeTrue();
            response.Methods.Single().Parameters.Count.ShouldBe(1);
            response.Methods.Single().Parameters.Single().Name.ShouldBe("builder");
            response.Methods.Single().Parameters.Single().TypeName.ShouldBe("MyClassBuilder<T>");
            response.Methods.Single().ReturnTypeName.ShouldBe("implicit");
        }

        [Fact]
        public async Task Adds_Operator_For_Abstract_Model_Without_Generics()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder()
                .WithAddImplicitOperatorOnBuilder(true)
                .WithEnableBuilderInheritance()
                .WithIsAbstract();
            var command = new GenerateBuilderCommand(CreateClass(), settings, CultureInfo.InvariantCulture);
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            response.Methods.Count.ShouldBe(1);
            response.Methods.Single().Name.ShouldBe("SomeNamespace.SomeClass");
            response.Methods.Single().Operator.ShouldBeTrue();
            response.Methods.Single().Parameters.Count.ShouldBe(1);
            response.Methods.Single().Parameters.Single().Name.ShouldBe("builder");
            response.Methods.Single().Parameters.Single().TypeName.ShouldBe("SomeClassBuilder<TBuilder, TEntity>");
            response.Methods.Single().ReturnTypeName.ShouldBe("implicit");
        }

        [Fact]
        public async Task Adds_Operator_For_Abstract_Model_With_Generics()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder()
                .WithAddImplicitOperatorOnBuilder(true)
                .WithEnableBuilderInheritance()
                .WithIsAbstract();
            var command = new GenerateBuilderCommand(CreateGenericClass(addProperties: false), settings, CultureInfo.InvariantCulture);
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            response.Methods.Count.ShouldBe(1);
            response.Methods.Single().Name.ShouldBe("MyNamespace.MyClass<T>");
            response.Methods.Single().Operator.ShouldBeTrue();
            response.Methods.Single().Parameters.Count.ShouldBe(1);
            response.Methods.Single().Parameters.Single().Name.ShouldBe("builder");
            response.Methods.Single().Parameters.Single().TypeName.ShouldBe("MyClassBuilder<TBuilder, TEntity, T>");
            response.Methods.Single().ReturnTypeName.ShouldBe("implicit");
        }

        [Fact]
        public async Task Adds_Operator_For_Non_Generic_Abstract_Model_Without_Generics()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder()
                .WithAddImplicitOperatorOnBuilder(true)
                .WithEnableBuilderInheritance()
                .WithIsAbstract()
                .WithIsForAbstractBuilder();
            var command = new GenerateBuilderCommand(CreateClass(), settings, CultureInfo.InvariantCulture);
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            response.Methods.Count.ShouldBe(1);
            response.Methods.Single().Name.ShouldBe("SomeNamespace.SomeClass");
            response.Methods.Single().Operator.ShouldBeTrue();
            response.Methods.Single().Parameters.Count.ShouldBe(1);
            response.Methods.Single().Parameters.Single().Name.ShouldBe("builder");
            response.Methods.Single().Parameters.Single().TypeName.ShouldBe("SomeClassBuilder");
            response.Methods.Single().ReturnTypeName.ShouldBe("implicit");
        }

        [Fact]
        public async Task Adds_Operator_For_Non_Generic_Abstract_Model_With_Generics()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder()
                .WithAddImplicitOperatorOnBuilder(true)
                .WithEnableBuilderInheritance()
                .WithIsAbstract()
                .WithIsForAbstractBuilder();
            var command = new GenerateBuilderCommand(CreateGenericClass(addProperties: false), settings, CultureInfo.InvariantCulture);
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            response.Methods.Count.ShouldBe(1);
            response.Methods.Single().Name.ShouldBe("MyNamespace.MyClass<T>");
            response.Methods.Single().Operator.ShouldBeTrue();
            response.Methods.Single().Parameters.Count.ShouldBe(1);
            response.Methods.Single().Parameters.Single().Name.ShouldBe("builder");
            response.Methods.Single().Parameters.Single().TypeName.ShouldBe("MyClassBuilder");
            response.Methods.Single().ReturnTypeName.ShouldBe("implicit");
        }

        [Fact]
        public async Task Adds_Operator_For_Override_Model_Without_Generics()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder()
                .WithAddImplicitOperatorOnBuilder(true)
                .WithEnableBuilderInheritance()
                .WithBaseClass(new ClassBuilder().WithName("Dummy"));
            var command = new GenerateBuilderCommand(CreateClass(), settings, CultureInfo.InvariantCulture);
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            response.Methods.Count.ShouldBe(1);
            response.Methods.Single().Name.ShouldBe("SomeNamespace.SomeClass");
            response.Methods.Single().Operator.ShouldBeTrue();
            response.Methods.Single().Parameters.Count.ShouldBe(1);
            response.Methods.Single().Parameters.Single().Name.ShouldBe("builder");
            response.Methods.Single().Parameters.Single().TypeName.ShouldBe("SomeClassBuilder");
            response.Methods.Single().ReturnTypeName.ShouldBe("implicit");
        }

        [Fact]
        public async Task Adds_Operator_For_Override_Model_With_Generics()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder()
                .WithAddImplicitOperatorOnBuilder(true)
                .WithEnableBuilderInheritance()
                .WithBaseClass(new ClassBuilder().WithName("Dummy"));
            var command = new GenerateBuilderCommand(CreateGenericClass(addProperties: false), settings, CultureInfo.InvariantCulture);
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            response.Methods.Count.ShouldBe(1);
            response.Methods.Single().Name.ShouldBe("MyNamespace.MyClass<T>");
            response.Methods.Single().Operator.ShouldBeTrue();
            response.Methods.Single().Parameters.Count.ShouldBe(1);
            response.Methods.Single().Parameters.Single().Name.ShouldBe("builder");
            response.Methods.Single().Parameters.Single().TypeName.ShouldBe("MyClassBuilder<T>");
            response.Methods.Single().ReturnTypeName.ShouldBe("implicit");
        }
    }
}

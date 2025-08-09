namespace ClassFramework.Pipelines.Tests.Builder.Components;

public class AddImplicitOperatorComponentTests : TestBase<Pipelines.Builder.Components.AddImplicitOperatorComponent>
{
    public class ProcessAsync : AddImplicitOperatorComponentTests
    {
        [Fact]
        public async Task Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            var t = sut.ProcessAsync(context: null!);
            (await Should.ThrowAsync<ArgumentNullException>(t))
             .ParamName.ShouldBe("context");
        }

        [Fact]
        public async Task Does_Not_Add_Operator_When_Setting_Is_False()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder().WithAddImplicitOperatorOnBuilder(false);
            var context = new PipelineContext<BuilderContext>(new BuilderContext(CreateClass(), settings, CultureInfo.InvariantCulture, CancellationToken.None));
            var sut = CreateSut();

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            context.Request.Builder.Methods.ShouldBeEmpty();
        }

        [Fact]
        public async Task Returns_ErrorResult_From_Parsing_Name()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder().WithAddImplicitOperatorOnBuilder(true);
            var context = new PipelineContext<BuilderContext>(new BuilderContext(CreateGenericClass(addProperties: false), settings, CultureInfo.InvariantCulture, CancellationToken.None));
            await InitializeExpressionEvaluatorAsync(forceError: true);
            var sut = CreateSut();

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Error);
            result.ErrorMessage.ShouldBe("Kaboom");
        }

        [Fact]
        public async Task Adds_Operator_For_Core_Model_Without_Generics()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder().WithAddImplicitOperatorOnBuilder(true);
            var context = new PipelineContext<BuilderContext>(new BuilderContext(CreateClass(), settings, CultureInfo.InvariantCulture, CancellationToken.None));
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            context.Request.Builder.Methods.Count.ShouldBe(1);
            context.Request.Builder.Methods.Single().Name.ShouldBe("SomeNamespace.SomeClass");
            context.Request.Builder.Methods.Single().Operator.ShouldBeTrue();
            context.Request.Builder.Methods.Single().Parameters.Count.ShouldBe(1);
            context.Request.Builder.Methods.Single().Parameters.Single().Name.ShouldBe("builder");
            context.Request.Builder.Methods.Single().Parameters.Single().TypeName.ShouldBe("SomeClassBuilder");
            context.Request.Builder.Methods.Single().ReturnTypeName.ShouldBe("implicit");
        }

        [Fact]
        public async Task Adds_Operator_For_Core_Model_With_Generics()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder().WithAddImplicitOperatorOnBuilder(true);
            var context = new PipelineContext<BuilderContext>(new BuilderContext(CreateGenericClass(addProperties: false), settings, CultureInfo.InvariantCulture, CancellationToken.None));
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            context.Request.Builder.Methods.Count.ShouldBe(1);
            context.Request.Builder.Methods.Single().Name.ShouldBe("MyNamespace.MyClass<T>");
            context.Request.Builder.Methods.Single().Operator.ShouldBeTrue();
            context.Request.Builder.Methods.Single().Parameters.Count.ShouldBe(1);
            context.Request.Builder.Methods.Single().Parameters.Single().Name.ShouldBe("builder");
            context.Request.Builder.Methods.Single().Parameters.Single().TypeName.ShouldBe("MyClassBuilder<T>");
            context.Request.Builder.Methods.Single().ReturnTypeName.ShouldBe("implicit");
        }

        [Fact]
        public async Task Adds_Operator_For_Abstract_Model_Without_Generics()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder()
                .WithAddImplicitOperatorOnBuilder(true)
                .WithEnableBuilderInheritance()
                .WithIsAbstract();
            var context = new PipelineContext<BuilderContext>(new BuilderContext(CreateClass(), settings, CultureInfo.InvariantCulture, CancellationToken.None));
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            context.Request.Builder.Methods.Count.ShouldBe(1);
            context.Request.Builder.Methods.Single().Name.ShouldBe("SomeNamespace.SomeClass");
            context.Request.Builder.Methods.Single().Operator.ShouldBeTrue();
            context.Request.Builder.Methods.Single().Parameters.Count.ShouldBe(1);
            context.Request.Builder.Methods.Single().Parameters.Single().Name.ShouldBe("builder");
            context.Request.Builder.Methods.Single().Parameters.Single().TypeName.ShouldBe("SomeClassBuilder<TBuilder, TEntity>");
            context.Request.Builder.Methods.Single().ReturnTypeName.ShouldBe("implicit");
        }

        [Fact]
        public async Task Adds_Operator_For_Abstract_Model_With_Generics()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder()
                .WithAddImplicitOperatorOnBuilder(true)
                .WithEnableBuilderInheritance()
                .WithIsAbstract();
            var context = new PipelineContext<BuilderContext>(new BuilderContext(CreateGenericClass(addProperties: false), settings, CultureInfo.InvariantCulture, CancellationToken.None));
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            context.Request.Builder.Methods.Count.ShouldBe(1);
            context.Request.Builder.Methods.Single().Name.ShouldBe("MyNamespace.MyClass<T>");
            context.Request.Builder.Methods.Single().Operator.ShouldBeTrue();
            context.Request.Builder.Methods.Single().Parameters.Count.ShouldBe(1);
            context.Request.Builder.Methods.Single().Parameters.Single().Name.ShouldBe("builder");
            context.Request.Builder.Methods.Single().Parameters.Single().TypeName.ShouldBe("MyClassBuilder<TBuilder, TEntity, T>");
            context.Request.Builder.Methods.Single().ReturnTypeName.ShouldBe("implicit");
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
            var context = new PipelineContext<BuilderContext>(new BuilderContext(CreateClass(), settings, CultureInfo.InvariantCulture, CancellationToken.None));
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            context.Request.Builder.Methods.Count.ShouldBe(1);
            context.Request.Builder.Methods.Single().Name.ShouldBe("SomeNamespace.SomeClass");
            context.Request.Builder.Methods.Single().Operator.ShouldBeTrue();
            context.Request.Builder.Methods.Single().Parameters.Count.ShouldBe(1);
            context.Request.Builder.Methods.Single().Parameters.Single().Name.ShouldBe("builder");
            context.Request.Builder.Methods.Single().Parameters.Single().TypeName.ShouldBe("SomeClassBuilder");
            context.Request.Builder.Methods.Single().ReturnTypeName.ShouldBe("implicit");
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
            var context = new PipelineContext<BuilderContext>(new BuilderContext(CreateGenericClass(addProperties: false), settings, CultureInfo.InvariantCulture, CancellationToken.None));
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            context.Request.Builder.Methods.Count.ShouldBe(1);
            context.Request.Builder.Methods.Single().Name.ShouldBe("MyNamespace.MyClass<T>");
            context.Request.Builder.Methods.Single().Operator.ShouldBeTrue();
            context.Request.Builder.Methods.Single().Parameters.Count.ShouldBe(1);
            context.Request.Builder.Methods.Single().Parameters.Single().Name.ShouldBe("builder");
            context.Request.Builder.Methods.Single().Parameters.Single().TypeName.ShouldBe("MyClassBuilder");
            context.Request.Builder.Methods.Single().ReturnTypeName.ShouldBe("implicit");
        }

        [Fact]
        public async Task Adds_Operator_For_Override_Model_Without_Generics()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder()
                .WithAddImplicitOperatorOnBuilder(true)
                .WithEnableBuilderInheritance()
                .WithBaseClass(new ClassBuilder().WithName("Dummy"));
            var context = new PipelineContext<BuilderContext>(new BuilderContext(CreateClass(), settings, CultureInfo.InvariantCulture, CancellationToken.None));
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            context.Request.Builder.Methods.Count.ShouldBe(1);
            context.Request.Builder.Methods.Single().Name.ShouldBe("SomeNamespace.SomeClass");
            context.Request.Builder.Methods.Single().Operator.ShouldBeTrue();
            context.Request.Builder.Methods.Single().Parameters.Count.ShouldBe(1);
            context.Request.Builder.Methods.Single().Parameters.Single().Name.ShouldBe("builder");
            context.Request.Builder.Methods.Single().Parameters.Single().TypeName.ShouldBe("SomeClassBuilder");
            context.Request.Builder.Methods.Single().ReturnTypeName.ShouldBe("implicit");
        }

        [Fact]
        public async Task Adds_Operator_For_Override_Model_With_Generics()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder()
                .WithAddImplicitOperatorOnBuilder(true)
                .WithEnableBuilderInheritance()
                .WithBaseClass(new ClassBuilder().WithName("Dummy"));
            var context = new PipelineContext<BuilderContext>(new BuilderContext(CreateGenericClass(addProperties: false), settings, CultureInfo.InvariantCulture, CancellationToken.None));
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
            context.Request.Builder.Methods.Count.ShouldBe(1);
            context.Request.Builder.Methods.Single().Name.ShouldBe("MyNamespace.MyClass<T>");
            context.Request.Builder.Methods.Single().Operator.ShouldBeTrue();
            context.Request.Builder.Methods.Single().Parameters.Count.ShouldBe(1);
            context.Request.Builder.Methods.Single().Parameters.Single().Name.ShouldBe("builder");
            context.Request.Builder.Methods.Single().Parameters.Single().TypeName.ShouldBe("MyClassBuilder<T>");
            context.Request.Builder.Methods.Single().ReturnTypeName.ShouldBe("implicit");
        }
    }
}

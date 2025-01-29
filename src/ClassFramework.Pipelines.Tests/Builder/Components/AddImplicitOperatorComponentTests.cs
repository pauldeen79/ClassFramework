namespace ClassFramework.Pipelines.Tests.Builder.Components;

public class AddImplicitOperatorComponentTests : TestBase<Pipelines.Builder.Components.AddImplicitOperatorComponent>
{
    public class ProcessAsync : AddImplicitOperatorComponentTests
    {
        [Fact]
        public void Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            sut.Awaiting(x => x.ProcessAsync(context: null!))
               .Should().ThrowAsync<ArgumentNullException>().WithParameterName("context");
        }

        [Fact]
        public async Task Does_Not_Add_Operator_When_Setting_Is_False()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder().WithAddImplicitOperatorOnBuilder(false);
            var context = new PipelineContext<BuilderContext>(new BuilderContext(CreateClass(), settings, CultureInfo.InvariantCulture));
            var sut = CreateSut();

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            context.Request.Builder.Methods.Should().BeEmpty();
        }

        [Fact]
        public async Task Returns_ErrorResult_From_Parsing_Name()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder().WithAddImplicitOperatorOnBuilder(true);
            var context = new PipelineContext<BuilderContext>(new BuilderContext(CreateGenericClass(addProperties: false), settings, CultureInfo.InvariantCulture));
            InitializeParser(forceError: true);
            var sut = CreateSut();

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Error);
            result.ErrorMessage.Should().Be("Kaboom");
        }

        [Fact]
        public async Task Adds_Operator_For_Core_Model_Without_Generics()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder().WithAddImplicitOperatorOnBuilder(true);
            var context = new PipelineContext<BuilderContext>(new BuilderContext(CreateClass(), settings, CultureInfo.InvariantCulture));
            InitializeParser();
            var sut = CreateSut();

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            context.Request.Builder.Methods.Should().ContainSingle();
            context.Request.Builder.Methods.Single().Name.Should().Be("SomeNamespace.SomeClass");
            context.Request.Builder.Methods.Single().Operator.Should().BeTrue();
            context.Request.Builder.Methods.Single().Parameters.Should().ContainSingle();
            context.Request.Builder.Methods.Single().Parameters.Single().Name.Should().Be("entity");
            context.Request.Builder.Methods.Single().Parameters.Single().TypeName.Should().Be("SomeClassBuilder");
            context.Request.Builder.Methods.Single().ReturnTypeName.Should().Be("implicit");
        }

        [Fact]
        public async Task Adds_Operator_For_Core_Model_With_Generics()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder().WithAddImplicitOperatorOnBuilder(true);
            var context = new PipelineContext<BuilderContext>(new BuilderContext(CreateGenericClass(addProperties: false), settings, CultureInfo.InvariantCulture));
            InitializeParser();
            var sut = CreateSut();

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            context.Request.Builder.Methods.Should().ContainSingle();
            context.Request.Builder.Methods.Single().Name.Should().Be("MyNamespace.MyClass<T>");
            context.Request.Builder.Methods.Single().Operator.Should().BeTrue();
            context.Request.Builder.Methods.Single().Parameters.Should().ContainSingle();
            context.Request.Builder.Methods.Single().Parameters.Single().Name.Should().Be("entity");
            context.Request.Builder.Methods.Single().Parameters.Single().TypeName.Should().Be("MyClassBuilder<T>");
            context.Request.Builder.Methods.Single().ReturnTypeName.Should().Be("implicit");
        }

        [Fact]
        public async Task Adds_Operator_For_Abstract_Model_Without_Generics()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder()
                .WithAddImplicitOperatorOnBuilder(true)
                .WithEnableBuilderInheritance()
                .WithIsAbstract();
            var context = new PipelineContext<BuilderContext>(new BuilderContext(CreateClass(), settings, CultureInfo.InvariantCulture));
            InitializeParser();
            var sut = CreateSut();

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            context.Request.Builder.Methods.Should().ContainSingle();
            context.Request.Builder.Methods.Single().Name.Should().Be("SomeNamespace.SomeClass");
            context.Request.Builder.Methods.Single().Operator.Should().BeTrue();
            context.Request.Builder.Methods.Single().Parameters.Should().ContainSingle();
            context.Request.Builder.Methods.Single().Parameters.Single().Name.Should().Be("entity");
            context.Request.Builder.Methods.Single().Parameters.Single().TypeName.Should().Be("SomeClassBuilder<TBuilder, TEntity>");
            context.Request.Builder.Methods.Single().ReturnTypeName.Should().Be("implicit");
        }

        [Fact]
        public async Task Adds_Operator_For_Abstract_Model_With_Generics()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder()
                .WithAddImplicitOperatorOnBuilder(true)
                .WithEnableBuilderInheritance()
                .WithIsAbstract();
            var context = new PipelineContext<BuilderContext>(new BuilderContext(CreateGenericClass(addProperties: false), settings, CultureInfo.InvariantCulture));
            InitializeParser();
            var sut = CreateSut();

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            context.Request.Builder.Methods.Should().ContainSingle();
            context.Request.Builder.Methods.Single().Name.Should().Be("MyNamespace.MyClass<T>");
            context.Request.Builder.Methods.Single().Operator.Should().BeTrue();
            context.Request.Builder.Methods.Single().Parameters.Should().ContainSingle();
            context.Request.Builder.Methods.Single().Parameters.Single().Name.Should().Be("entity");
            context.Request.Builder.Methods.Single().Parameters.Single().TypeName.Should().Be("MyClassBuilder<TBuilder, TEntity, T>");
            context.Request.Builder.Methods.Single().ReturnTypeName.Should().Be("implicit");
        }
    }
}

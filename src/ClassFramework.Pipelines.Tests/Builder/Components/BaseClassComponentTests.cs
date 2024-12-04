namespace ClassFramework.Pipelines.Tests.Builder.Components;

public class BaseClassComponentTests : TestBase<Pipelines.Builder.Components.BaseClassComponent>
{
    public class Process : BaseClassComponentTests
    {
        [Fact]
        public void Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            sut.Awaiting(x => x.Process(context: null!))
               .Should().ThrowAsync<ArgumentNullException>().WithParameterName("context");
        }

        [Fact]
        public async Task Sets_BaseClass_For_BuilderInheritance_And_Not_For_Abstract_Builder_No_BaseClass()
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                enableBuilderInheritance: true,
                baseClass: null,
                enableEntityInheritance: true);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.BaseClass.Should().Be("SomeClassBuilder");
        }

        [Fact]
        public async Task Sets_BaseClass_For_BuilderInheritance_And_Not_For_Abstract_Builder_With_BaseClass_And_Abstract()
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                enableBuilderInheritance: true,
                baseClass: new ClassBuilder().WithName("BaseClass").BuildTyped(),
                isAbstract: true,
                enableEntityInheritance: true);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.BaseClass.Should().Be("SomeClassBuilder");
        }

        [Fact]
        public async Task Sets_BaseClass_For_BuilderInheritance_And_Not_For_Abstract_Builder_With_BaseClass_And_Not_Abstract_No_Builders_Namespace()
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                enableBuilderInheritance: true,
                baseClass: new ClassBuilder().WithName("BaseClass").BuildTyped(),
                isAbstract: false,
                enableEntityInheritance: true);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.BaseClass.Should().Be("BaseClassBuilder<SomeClassBuilder, SomeNamespace.SomeClass>");
        }

        [Fact]
        public async Task Sets_BaseClass_For_BuilderInheritance_And_Not_For_Abstract_Builder_With_BaseClass_And_Not_Abstract_Builders_Namespace()
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                enableBuilderInheritance: true,
                baseClass: new ClassBuilder().WithName("BaseClass").BuildTyped(),
                isAbstract: false,
                baseClassBuilderNameSpace: "BaseBuilders",
                enableEntityInheritance: true);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.BaseClass.Should().Be("BaseBuilders.BaseClassBuilder<SomeClassBuilder, SomeNamespace.SomeClass>");
        }

        [Fact]
        public async Task Sets_BaseClass_For_BuilderInheritance_For_Abstract_Builder_With_BaseClass()
        {
            // Arrange
            var sourceModel = CreateClass("MyBaseClass");
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                enableBuilderInheritance: true,
                enableEntityInheritance: true).WithIsForAbstractBuilder();
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.BaseClass.Should().Be("MyBaseClassBuilder");
        }

        [Fact]
        public async Task Sets_BaseClass_For_No_BuilderInheritance_For_Abstract_Builder_With_BaseClass()
        {
            // Arrange
            var sourceModel = CreateClass("MyBaseClass");
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                enableBuilderInheritance: false,
                enableEntityInheritance: true).WithIsForAbstractBuilder();
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.BaseClass.Should().Be("MyBaseClassBuilder<SomeClassBuilder, SomeNamespace.SomeClass>");
        }

        [Fact]
        public async Task Returns_Error_When_Parsing_BuilderNameFormatString_Is_Not_Succesful()
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                enableBuilderInheritance: true,
                baseClass: null,
                enableEntityInheritance: true,
                builderNameFormatString: "{Error}");
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.Process(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Error);
            result.ErrorMessage.Should().Be("Kaboom");
        }

        private static PipelineContext<BuilderContext> CreateContext(TypeBase sourceModel, PipelineSettingsBuilder settings)
            => new(new BuilderContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));
    }
}

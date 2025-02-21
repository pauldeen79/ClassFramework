namespace ClassFramework.Pipelines.Tests.Builder.Components;

public class BaseClassComponentTests : TestBase<Pipelines.Builder.Components.BaseClassComponent>
{
    public class ProcessAsync : BaseClassComponentTests
    {
        [Fact]
        public async Task Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            Task t = sut.ProcessAsync(context: null!);
            (await t.ShouldThrowAsync<ArgumentNullException>()).ParamName.ShouldBe("context");
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
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.BaseClass.ShouldBe("SomeClassBuilder");
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
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.BaseClass.ShouldBe("SomeClassBuilder");
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
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.BaseClass.ShouldBe("BaseClassBuilder<SomeClassBuilder, SomeNamespace.SomeClass>");
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
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.BaseClass.ShouldBe("BaseBuilders.BaseClassBuilder<SomeClassBuilder, SomeNamespace.SomeClass>");
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
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.BaseClass.ShouldBe("MyBaseClassBuilder");
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
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.BaseClass.ShouldBe("MyBaseClassBuilder<SomeClassBuilder, SomeNamespace.SomeClass>");
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
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Error);
            result.ErrorMessage.ShouldBe("Kaboom");
        }

        private static PipelineContext<BuilderContext> CreateContext(TypeBase sourceModel, PipelineSettingsBuilder settings)
            => new(new BuilderContext(sourceModel, settings, CultureInfo.InvariantCulture));
    }
}

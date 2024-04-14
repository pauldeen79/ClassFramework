namespace ClassFramework.Pipelines.Tests.Builder.Components;

public class AbstractBuilderComponentTests : TestBase<Pipelines.Builder.Features.AbstractBuilderComponent>
{
    public class Process : AbstractBuilderComponentTests
    {
        [Fact]
        public async Task Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            await sut.Awaiting(x => x.Process(context: null!, CancellationToken.None))
                     .Should().ThrowAsync<ArgumentNullException>().WithParameterName("context");
        }

        [Fact]
        public async Task Adds_AddGenericTypeArguments_When_IsBuilderForAbstractEntity_Is_True()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").Build();
            InitializeParser();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForBuilder(enableEntityInheritance: true);
            var context = CreateContext(sourceModel, model, settings);

            // Act
            var result = await sut.Process(context, CancellationToken.None);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.GenericTypeArguments.Should().BeEquivalentTo("TBuilder", "TEntity");
            model.GenericTypeArgumentConstraints.Should().BeEquivalentTo("where TEntity : SomeNamespace.SomeClass", "where TBuilder : SomeClassBuilder<TBuilder, TEntity>");
            model.Abstract.Should().BeTrue();
        }

        [Fact]
        public async Task Does_Not_Add_AddGenericTypeArguments_When_IsBuilderForAbstractEntity_Is_False_And_Validation_Is_Not_Shared_Between_Builder_And_Entity()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").Build();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForBuilder();
            var context = CreateContext(sourceModel, model, settings);

            // Act
            var result = await sut.Process(context, CancellationToken.None);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.GenericTypeArguments.Should().BeEmpty();
            model.GenericTypeArgumentConstraints.Should().BeEmpty();
            model.Abstract.Should().BeFalse();
        }

        [Fact]
        public async Task Returns_Error_When_Parsing_NameFormatString_Is_Not_Successful()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").Build();
            InitializeParser();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForBuilder(enableEntityInheritance: true, builderNameFormatString: "{Error}");
            var context = CreateContext(sourceModel, model, settings);

            // Act
            var result = await sut.Process(context, CancellationToken.None);

            // Assert
            result.Status.Should().Be(ResultStatus.Error);
            result.ErrorMessage.Should().Be("Kaboom");
        }

        [Fact]
        public async Task Returns_Invalid_When_Model_Is_Not_Of_Type_ClassBuilder()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").Build();
            InitializeParser();
            var sut = CreateSut();
            var model = new StructBuilder();
            var settings = CreateSettingsForBuilder(enableEntityInheritance: true);
            var context = CreateContext(sourceModel, model, settings);

            // Act
            var result = await sut.Process(context, CancellationToken.None);

            // Assert
            result.Status.Should().Be(ResultStatus.Invalid);
            result.ErrorMessage.Should().Be("You can only create abstract classes. The type of model (ClassFramework.Domain.Builders.Types.StructBuilder) is not a ClassBuilder");
        }

        private static PipelineContext<IConcreteTypeBuilder, BuilderContext> CreateContext(TypeBase sourceModel, IConcreteTypeBuilder model, PipelineSettingsBuilder settings)
            => new(model, new BuilderContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));
    }
}

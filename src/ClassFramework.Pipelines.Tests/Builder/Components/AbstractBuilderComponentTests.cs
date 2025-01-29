namespace ClassFramework.Pipelines.Tests.Builder.Components;

public class AbstractBuilderComponentTests : TestBase<Pipelines.Builder.Components.AbstractBuilderComponent>
{
    public class Process : AbstractBuilderComponentTests
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
        public async Task Adds_AddGenericTypeArguments_When_IsBuilderForAbstractEntity_Is_True()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").Build();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(enableEntityInheritance: true);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.GenericTypeArguments.Should().BeEquivalentTo("TBuilder", "TEntity");
            context.Request.Builder.GenericTypeArgumentConstraints.Should().BeEquivalentTo("where TEntity : SomeNamespace.SomeClass", "where TBuilder : SomeClassBuilder<TBuilder, TEntity>");
            context.Request.Builder.Abstract.Should().BeTrue();
        }

        [Fact]
        public async Task Does_Not_Add_AddGenericTypeArguments_When_IsBuilderForAbstractEntity_Is_False_And_Validation_Is_Not_Shared_Between_Builder_And_Entity()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").Build();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder();
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.GenericTypeArguments.Should().BeEmpty();
            context.Request.Builder.GenericTypeArgumentConstraints.Should().BeEmpty();
            context.Request.Builder.Abstract.Should().BeFalse();
        }

        [Fact]
        public async Task Returns_Error_When_Parsing_NameFormatString_Is_Not_Successful()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").Build();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(enableEntityInheritance: true, builderNameFormatString: "{Error}");
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Error);
            result.ErrorMessage.Should().Be("Kaboom");
        }

        private static PipelineContext<BuilderContext> CreateContext(TypeBase sourceModel, PipelineSettingsBuilder settings)
            => new(new BuilderContext(sourceModel, settings, CultureInfo.InvariantCulture));
    }
}

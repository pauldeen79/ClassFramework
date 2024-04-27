namespace ClassFramework.Pipelines.Tests.Builder.Components;

public class AddAttributesComponentTests : TestBase<Pipelines.Builder.Components.AddAttributesComponent>
{
    public class Process : AddAttributesComponentTests
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
        public async Task Adds_Attributes_When_CopyAttributes_Setting_Is_True()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").AddAttributes(new AttributeBuilder().WithName("MyAttribute")).Build();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForBuilder(copyAttributes: true);
            var context = CreateContext(sourceModel, model, settings);

            // Act
            var result = await sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.Attributes.Should().BeEquivalentTo(new[] { new AttributeBuilder().WithName("MyAttribute") });
        }

        [Fact]
        public async Task Adds_Filtered_Attributes_When_CopyAttributes_Setting_Is_True_And_Predicate_Is_Filled()
        {
            // Arrange
            var sourceModel = new ClassBuilder()
                .WithName("SomeClass")
                .WithNamespace("SomeNamespace")
                .AddAttributes(
                    new AttributeBuilder().WithName("MyAttribute1"),
                    new AttributeBuilder().WithName("MyAttribute2"))
                .Build();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForBuilder(copyAttributes: true, copyAttributePredicate: x => x.Name == "MyAttribute2");
            var context = CreateContext(sourceModel, model, settings);

            // Act
            var result = await sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.Attributes.Should().BeEquivalentTo(new[] { new AttributeBuilder().WithName("MyAttribute2") });
        }

        [Fact]
        public async Task Does_Not_Add_Attributes_When_CopyAttributes_Setting_Is_False()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").AddAttributes(new AttributeBuilder().WithName("MyAttribute")).Build();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForBuilder(copyAttributes: false);
            var context = CreateContext(sourceModel, model, settings);

            // Act
            var result = await sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.Attributes.Should().BeEmpty();
        }

        private static PipelineContext<BuilderContext, IConcreteTypeBuilder> CreateContext(TypeBase sourceModel, ClassBuilder model, PipelineSettingsBuilder settings)
            => new(new BuilderContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture), model);
    }
}

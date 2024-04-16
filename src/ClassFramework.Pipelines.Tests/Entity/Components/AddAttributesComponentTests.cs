namespace ClassFramework.Pipelines.Tests.Entity.Components;

public class AddAttributesComponentTests : TestBase<Pipelines.Entity.Features.AddAttributesComponent>
{
    public class Process : AddAttributesComponentTests
    {
        [Fact]
        public async Task Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            await sut.Awaiting(x => x.Process(context: null!))
                     .Should().ThrowAsync<ArgumentNullException>().WithParameterName("context");
        }

        [Fact]
        public async Task Adds_Attributes_When_CopyAttributePredicate_Setting_Is_Not_Null_And_CopyAttributes_Is_True()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").AddAttributes(new AttributeBuilder().WithName("MyAttribute")).BuildTyped();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForEntity(copyAttributePredicate: _ => true, copyAttributes: true);
            var context = new PipelineContext<IConcreteTypeBuilder, EntityContext>(model, new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.Attributes.Should().BeEquivalentTo(new[] { new AttributeBuilder().WithName("MyAttribute") });
        }

        [Fact]
        public async Task Adds_Attributes_When_CopyAttributePredicate_Setting_Is_Null_And_CopyAttributes_Is_True()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").AddAttributes(new AttributeBuilder().WithName("MyAttribute")).BuildTyped();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForEntity(copyAttributePredicate: null, copyAttributes: true);
            var context = new PipelineContext<IConcreteTypeBuilder, EntityContext>(model, new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.Attributes.Should().BeEquivalentTo(new[] { new AttributeBuilder().WithName("MyAttribute") });
        }

        [Fact]
        public async Task Does_Not_Copy_Attributes_When_CopyAttributes_Is_False()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").AddAttributes(new AttributeBuilder().WithName("MyAttribute")).BuildTyped();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForEntity(copyAttributes: false);
            var context = new PipelineContext<IConcreteTypeBuilder, EntityContext>(model, new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.Attributes.Should().BeEmpty();
        }
    }
}

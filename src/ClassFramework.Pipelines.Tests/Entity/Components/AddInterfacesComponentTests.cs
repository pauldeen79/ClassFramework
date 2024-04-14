namespace ClassFramework.Pipelines.Tests.Entity.Components;

public class AddInterfacesComponentTests : TestBase<Pipelines.Entity.Features.AddInterfacesComponent>
{
    public class Process : AddInterfacesComponentTests
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
        public async Task Adds_Interfaces_When_CopyInterfacePredicate_Setting_Is_Not_Null_And_CopyAttributes_Is_True()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").AddInterfaces("IMyInterface").BuildTyped();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForEntity(copyInterfacePredicate: _ => true, copyInterfaces: true);
            var context = new PipelineContext<IConcreteTypeBuilder, EntityContext>(model, new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.Process(context, CancellationToken.None);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.Interfaces.Should().BeEquivalentTo("IMyInterface");
        }

        [Fact]
        public async Task Adds_Interfaces_When_CopyInterfacePredicate_Setting_Is_Null_And_CopyAttributes_Is_True()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").AddInterfaces("IMyInterface").BuildTyped();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForEntity(copyInterfacePredicate: null, copyInterfaces: true);
            var context = new PipelineContext<IConcreteTypeBuilder, EntityContext>(model, new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.Process(context, CancellationToken.None);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.Interfaces.Should().BeEquivalentTo("IMyInterface");
        }

        [Fact]
        public async Task Does_Not_Add_Interfaces_When_And_CopyAttributes_Is_False()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").AddInterfaces("IMyInterface").BuildTyped();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForEntity(copyInterfaces: false);
            var context = new PipelineContext<IConcreteTypeBuilder, EntityContext>(model, new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.Process(context, CancellationToken.None);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.Interfaces.Should().BeEmpty();
        }
    }
}

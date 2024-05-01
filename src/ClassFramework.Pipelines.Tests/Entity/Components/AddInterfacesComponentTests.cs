namespace ClassFramework.Pipelines.Tests.Entity.Components;

public class AddInterfacesComponentTests : TestBase<Pipelines.Entity.Components.AddInterfacesComponent>
{
    public class Process : AddInterfacesComponentTests
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
        public async Task Adds_Interfaces_When_CopyInterfacePredicate_Setting_Is_Not_Null_And_CopyAttributes_Is_True()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").AddInterfaces("IMyInterface").BuildTyped();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(copyInterfacePredicate: _ => true, copyInterfaces: true);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Interfaces.Should().BeEquivalentTo("IMyInterface");
        }

        [Fact]
        public async Task Adds_Interfaces_When_CopyInterfacePredicate_Setting_Is_Null_And_CopyAttributes_Is_True()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").AddInterfaces("IMyInterface").BuildTyped();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(copyInterfacePredicate: null, copyInterfaces: true);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Interfaces.Should().BeEquivalentTo("IMyInterface");
        }

        [Fact]
        public async Task Does_Not_Add_Interfaces_When_And_CopyAttributes_Is_False()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").AddInterfaces("IMyInterface").BuildTyped();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(copyInterfaces: false);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Interfaces.Should().BeEmpty();
        }
    }
}

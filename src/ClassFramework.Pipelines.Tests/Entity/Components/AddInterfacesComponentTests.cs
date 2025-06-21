namespace ClassFramework.Pipelines.Tests.Entity.Components;

public class AddInterfacesComponentTests : TestBase<Pipelines.Entity.Components.AddInterfacesComponent>
{
    public class ProcessAsync : AddInterfacesComponentTests
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
        public async Task Adds_Interfaces_When_CopyInterfacePredicate_Setting_Is_Not_Null_And_CopyAttributes_Is_True()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").AddInterfaces("IMyInterface").BuildTyped();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(copyInterfacePredicate: _ => true, copyInterfaces: true);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Interfaces.ToArray().ShouldBeEquivalentTo(new[] { "IMyInterface" });
        }

        [Fact]
        public async Task Adds_Interfaces_When_CopyInterfacePredicate_Setting_Is_Null_And_CopyAttributes_Is_True()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").AddInterfaces("IMyInterface").BuildTyped();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(copyInterfacePredicate: null, copyInterfaces: true);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Interfaces.ToArray().ShouldBeEquivalentTo(new[] { "IMyInterface" });
        }

        [Fact]
        public async Task Does_Not_Add_Interfaces_When_And_CopyAttributes_Is_False()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").AddInterfaces("IMyInterface").BuildTyped();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(copyInterfaces: false);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Interfaces.ShouldBeEmpty();
        }
    }
}

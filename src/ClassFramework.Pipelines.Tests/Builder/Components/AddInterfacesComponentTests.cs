namespace ClassFramework.Pipelines.Tests.Builder.Components;

public class AddInterfacesComponentTests : TestBase<Pipelines.Builder.Components.AddInterfacesComponent>
{
    public class ExecuteAsync : AddInterfacesComponentTests
    {
        [Fact]
        public async Task Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            var t = sut.ExecuteAsync(context: null!, CommandService, CancellationToken.None);
            (await Should.ThrowAsync<ArgumentNullException>(t))
             .ParamName.ShouldBe("context");
        }

        [Fact]
        public async Task Adds_Interfaces_When_CopyInterfaces_Setting_Is_True()
        {
            // Arrange
            var sourceModel = new ClassBuilder()
                .WithName("SomeClass")
                .WithNamespace("SomeNamespace")
                .AddInterfaces("IMyInterface")
                .Build();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(copyInterfaces: true);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ExecuteAsync(context, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Builder.Interfaces.ToArray().ShouldBeEquivalentTo(new[] { "IMyInterface" });
        }

        [Fact]
        public async Task Adds_Filtered_Interfaces_When_CopyInterfaces_Setting_Is_True_And_Predicate_Is_Filled()
        {
            // Arrange
            var sourceModel = new ClassBuilder()
                .WithName("SomeClass")
                .WithNamespace("SomeNamespace")
                .AddInterfaces(
                    "IMyInterface1",
                    "IMyInterface2")
                .Build();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(copyInterfaces: true, copyInterfacePredicate: x => x == "IMyInterface2");
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ExecuteAsync(context, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Builder.Interfaces.ToArray().ShouldBeEquivalentTo(new[] { "IMyInterface2" });
        }

        [Fact]
        public async Task Does_Not_Add_Interfaces_When_CopyInterfaces_Setting_Is_False()
        {
            // Arrange
            var sourceModel = new ClassBuilder()
                .WithName("SomeClass")
                .WithNamespace("SomeNamespace")
                .AddInterfaces("IMyInterface")
                .Build();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(copyInterfaces: false);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ExecuteAsync(context, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Builder.Interfaces.ShouldBeEmpty();
        }

        private static BuilderContext CreateContext(TypeBase sourceModel, PipelineSettingsBuilder settings)
            => new BuilderContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None);
    }
}

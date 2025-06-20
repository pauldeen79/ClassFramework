namespace ClassFramework.Pipelines.Tests.Builder.Components;

public class AddInterfacesComponentTests : TestBase<Pipelines.Builder.Components.AddInterfacesComponent>
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
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Interfaces.ToArray().ShouldBeEquivalentTo(new[] { "IMyInterface" });
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
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Interfaces.ToArray().ShouldBeEquivalentTo(new[] { "IMyInterface2" });
        }

        [Fact]
        public async Task Only_Adds_IBuilder_Interface_When_CopyInterfaces_Setting_Is_False()
        {
            // Arrange
            var sourceModel = new ClassBuilder()
                .WithName("SomeClass")
                .WithNamespace("SomeNamespace")
                .AddInterfaces("IMyInterface")
                .Build();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(copyInterfaces: false, useCrossCuttingInterfaces: true);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Interfaces.ToArray().ShouldBeEquivalentTo(new string[] { "CrossCutting.Common.Abstractions.IBuilder<SomeNamespace.SomeClass>" });
        }

        private static PipelineContext<BuilderContext> CreateContext(TypeBase sourceModel, PipelineSettingsBuilder settings)
            => new(new BuilderContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None));
    }
}

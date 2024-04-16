namespace ClassFramework.Pipelines.Tests.Builder.Components;

public class AddInterfacesComponentTests : TestBase<Pipelines.Builder.Features.AddInterfacesComponent>
{
    public class Process : AddInterfacesComponentTests
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
        public async Task Adds_Interfaces_When_CopyInterfaces_Setting_Is_True()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").AddInterfaces("IMyInterface").Build();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForBuilder(copyInterfaces: true);
            var context = CreateContext(sourceModel, model, settings);

            // Act
            var result = await sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.Interfaces.Should().BeEquivalentTo("IMyInterface");
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
            var model = new ClassBuilder();
            var settings = CreateSettingsForBuilder(copyInterfaces: true, copyInterfacePredicate: x => x == "IMyInterface2");
            var context = CreateContext(sourceModel, model, settings);

            // Act
            var result = await sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.Interfaces.Should().BeEquivalentTo("IMyInterface2");
        }

        [Fact]
        public async Task Does_Not_Add_Interfaces_When_CopyInterfaces_Setting_Is_False()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").AddInterfaces("IMyInterface").Build();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForBuilder(copyInterfaces: false);
            var context = CreateContext(sourceModel, model, settings);

            // Act
            var result = await sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.Interfaces.Should().BeEmpty();
        }

        private static PipelineContext<IConcreteTypeBuilder, BuilderContext> CreateContext(TypeBase sourceModel, ClassBuilder model, PipelineSettingsBuilder settings)
            => new(model, new BuilderContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));
    }
}

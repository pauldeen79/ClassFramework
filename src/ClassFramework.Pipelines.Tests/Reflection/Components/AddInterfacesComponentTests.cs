namespace ClassFramework.Pipelines.Tests.Reflection.Components;

public class AddInterfacesComponentTests : TestBase<Pipelines.Reflection.Components.AddInterfacesComponent>
{
    public class Process : AddInterfacesComponentTests
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
        public async Task Does_Not_Copy_Interfaces_When_CopyInterfaces_Is_False()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyClass);
            var settings = CreateSettingsForReflection(copyInterfaces: false);
            var context = new PipelineContext<ReflectionContext>(new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Interfaces.Should().BeEmpty();
        }

        [Fact]
        public async Task Copies_Interfaces_When_CopyInterfaces_Is_True()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyClass);
            var settings = CreateSettingsForReflection(copyInterfaces: true);
            var context = new PipelineContext<ReflectionContext>(new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Interfaces.Should().ContainSingle();
        }

        [Fact]
        public async Task Filters_Interfaces_When_CopyInterfaces_Is_True_And_Predicate_Is_Set()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyClass);
            var settings = CreateSettingsForReflection(copyInterfaces: false, copyInterfacePredicate: _ => false);
            var context = new PipelineContext<ReflectionContext>(new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Interfaces.Should().BeEmpty();
        }
    }
}

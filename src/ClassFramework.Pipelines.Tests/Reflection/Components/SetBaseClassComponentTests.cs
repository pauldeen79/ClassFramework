namespace ClassFramework.Pipelines.Tests.Reflection.Components;

public class SetBaseClassComponentTests : TestBase<Pipelines.Reflection.Components.SetBaseClassComponent>
{
    public class Process : SetBaseClassComponentTests
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
        public async Task Sets_BaseClass_When_Available_Using_SourceModel_BaseClass()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyBaseClassTestClass);
            var settings = CreateSettingsForReflection();
            var context = new PipelineContext<ReflectionContext>(new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            ((ClassBuilder)context.Request.Builder).BaseClass.Should().Be("ClassFramework.Pipelines.Tests.Reflection.Components.MyBaseClassTestClassBase");
        }

        [Fact]
        public async Task Does_Not_Set_BaseClass_When_Not_Available_Using_SourceModel_BaseClass()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyClass);
            var settings = CreateSettingsForReflection();
            var context = new PipelineContext<ReflectionContext>(new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));
            ((ClassBuilder)context.Request.Builder).WithBaseClass("Old value");

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            ((ClassBuilder)context.Request.Builder).BaseClass.Should().BeEmpty();
        }

        [Fact]
        public async Task Sets_BaseClass_When_Available_Using_Inheritance_From_Settings()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyClass);
            var settings = CreateSettingsForReflection(
                useBaseClassFromSourceModel: false,
                enableEntityInheritance: true,
                baseClass: new ClassBuilder().WithName("MyBaseClass").BuildTyped());
            var context = new PipelineContext<ReflectionContext>(new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            ((ClassBuilder)context.Request.Builder).BaseClass.Should().Be("MyBaseClass");
        }

        [Fact]
        public async Task Does_Not_Set_BaseClass_When_Available_Using_Inheritance_From_Settings_When_BaseClass_Is_Empty()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyClass);
            var settings = CreateSettingsForReflection(
                useBaseClassFromSourceModel: false,
                enableEntityInheritance: true,
                baseClass: null);
            var context = new PipelineContext<ReflectionContext>(new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            ((ClassBuilder)context.Request.Builder).BaseClass.Should().BeEmpty();
        }

        [Fact]
        public async Task Does_Not_Set_BaseClass_When_UseBaseClassFromSource_Is_False()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyBaseClassTestClass);
            var settings = CreateSettingsForReflection(useBaseClassFromSourceModel: false);
            var context = new PipelineContext<ReflectionContext>(new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));
            ((ClassBuilder)context.Request.Builder).WithBaseClass("Old value");

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            ((ClassBuilder)context.Request.Builder).BaseClass.Should().BeEmpty();
        }
    }
}

#pragma warning disable CA1812 // Avoid uninstantiated internal classes
#pragma warning disable S2094 // Classes should not be empty
internal sealed class MyBaseClassTestClass : MyBaseClassTestClassBase
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
#pragma warning restore S2094 // Classes should not be empty
{
}

#pragma warning disable CA1812 // Avoid uninstantiated internal classes
#pragma warning disable S2094 // Classes should not be empty
internal class MyBaseClassTestClassBase
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
#pragma warning restore S2094 // Classes should not be empty
{
}

namespace ClassFramework.Pipelines.Tests.Reflection.Components;

public class SetBaseClassComponentTests : TestBase<Pipelines.Reflection.Components.SetBaseClassComponent>
{
    public class ExecuteAsync : SetBaseClassComponentTests
    {
        [Fact]
        public async Task Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();
            var response = new ClassBuilder();

            // Act & Assert
            Task a = sut.ExecuteAsync(context: null!, response, CommandService, CancellationToken.None);
            (await a.ShouldThrowAsync<ArgumentNullException>())
                .ParamName.ShouldBe("context");
        }

        [Fact]
        public async Task Sets_BaseClass_When_Available_Using_SourceModel_BaseClass()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyBaseClassTestClass);
            var settings = CreateSettingsForReflection();
            var command = new GenerateTypeFromReflectionCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            ((ClassBuilder)response).BaseClass.ShouldBe("ClassFramework.Pipelines.Tests.Reflection.Components.MyBaseClassTestClassBase");
        }

        [Fact]
        public async Task Does_Not_Set_BaseClass_When_Not_Available_Using_SourceModel_BaseClass()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyClass);
            var settings = CreateSettingsForReflection();
            var command = new GenerateTypeFromReflectionCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder().WithBaseClass("Old value");

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.BaseClass.ShouldBeEmpty();
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
            var command = new GenerateTypeFromReflectionCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.BaseClass.ShouldBe("MyBaseClass");
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
            var command = new GenerateTypeFromReflectionCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.BaseClass.ShouldBeEmpty();
        }

        [Fact]
        public async Task Does_Not_Set_BaseClass_When_UseBaseClassFromSource_Is_False()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyBaseClassTestClass);
            var settings = CreateSettingsForReflection(useBaseClassFromSourceModel: false);
            var command = new GenerateTypeFromReflectionCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder().WithBaseClass("Old value");

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.BaseClass.ShouldBeEmpty();
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

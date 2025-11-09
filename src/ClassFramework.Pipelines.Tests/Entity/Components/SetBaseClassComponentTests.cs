namespace ClassFramework.Pipelines.Tests.Entity.Components;

public class SetBaseClassComponentTests : TestBase<Pipelines.Entity.Components.SetBaseClassComponent>
{
    public class ExecuteAsync : SetBaseClassComponentTests
    {
        [Fact]
        public async Task Throws_On_Null_Command()
        {
            // Arrange
            var sut = CreateSut();
            var response = new ClassBuilder();

            // Act & Assert
            var t = sut.ExecuteAsync(command: null!, response, CommandService, CancellationToken.None);
            (await Should.ThrowAsync<ArgumentNullException>(t))
             .ParamName.ShouldBe("command");
        }

        [Fact]
        public async Task Does_Not_Set_BaseClass_For_EntityInheritance_When_SourceModel_And_EntitySettings_Do_Not_Have_A_BaseClass()
        {
            // Arrange
            var sourceModel = CreateClass(baseClass: string.Empty);
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(
                baseClass: null,
                enableEntityInheritance: true);
            var command = new GenerateEntityCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.BaseClass.ShouldBeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData("ThisBaseClassGetsIgnored")]
        public async Task Sets_BaseClass_For_EntityInheritance_From_EntitySettings_When_Specified(string sourceModelBaseClass)
        {
            // Arrange
            var sourceModel = CreateClass(baseClass: sourceModelBaseClass);
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(
                baseClass: new ClassBuilder().WithName("MyBaseClass").WithNamespace("MyBaseNamespace").BuildTyped(),
                enableEntityInheritance: true);
            var command = new GenerateEntityCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.BaseClass.ShouldBe("MyBaseNamespace.MyBaseClass");
        }

        [Fact]
        public async Task Sets_BaseClass_For_EntityInheritance_From_Source_When_Specified()
        {
            // Arrange
            var sourceModel = CreateClass(baseClass: "MyBaseNamespace.MyBaseClass");
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(
                baseClass: null,
                enableEntityInheritance: true);
            var command = new GenerateEntityCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.BaseClass.ShouldBe("MyBaseNamespace.MyBaseClass");
        }
    }
}

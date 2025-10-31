namespace ClassFramework.Pipelines.Tests.Entity.Components;

public class SetBaseClassComponentTests : TestBase<Pipelines.Entity.Components.SetBaseClassComponent>
{
    public class ExecuteAsync : SetBaseClassComponentTests
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
        public async Task Does_Not_Set_BaseClass_For_EntityInheritance_When_SourceModel_And_EntitySettings_Do_Not_Have_A_BaseClass()
        {
            // Arrange
            var sourceModel = CreateClass(baseClass: string.Empty);
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(
                baseClass: null,
                enableEntityInheritance: true);
            var context = new EntityContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None);

            // Act
            var result = await sut.ExecuteAsync(context, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Builder.BaseClass.ShouldBeEmpty();
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
            var context = new EntityContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None);

            // Act
            var result = await sut.ExecuteAsync(context, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Builder.BaseClass.ShouldBe("MyBaseNamespace.MyBaseClass");
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
            var context = new EntityContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None);

            // Act
            var result = await sut.ExecuteAsync(context, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Builder.BaseClass.ShouldBe("MyBaseNamespace.MyBaseClass");
        }
    }
}

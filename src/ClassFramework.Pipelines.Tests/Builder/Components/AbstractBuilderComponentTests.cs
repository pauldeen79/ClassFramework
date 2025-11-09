namespace ClassFramework.Pipelines.Tests.Builder.Components;

public class AbstractBuilderComponentTests : TestBase<Pipelines.Builder.Components.AbstractBuilderComponent>
{
    public class ExecuteAsync : AbstractBuilderComponentTests
    {
        [Fact]
        public async Task Throws_On_Null_Command()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            var t = sut.ExecuteAsync(command: null!, new ClassBuilder(), CommandService, CancellationToken.None);
            (await Should.ThrowAsync<ArgumentNullException>(t))
             .ParamName.ShouldBe("command");
        }

        [Fact]
        public async Task Adds_AddGenericTypeArguments_When_IsBuilderForAbstractEntity_Is_True()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").Build();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(enableEntityInheritance: true);
            var command = CreateCommand(sourceModel, settings);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.GenericTypeArguments.ToArray().ShouldBeEquivalentTo(new[] { "TBuilder", "TEntity" });
            response.GenericTypeArgumentConstraints.ToArray().ShouldBeEquivalentTo(new[] { "where TEntity : SomeNamespace.SomeClass", "where TBuilder : SomeClassBuilder<TBuilder, TEntity>" });
            response.Abstract.ShouldBeTrue();
        }

        [Fact]
        public async Task Does_Not_Add_AddGenericTypeArguments_When_IsBuilderForAbstractEntity_Is_False_And_Validation_Is_Not_Shared_Between_Builder_And_Entity()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").Build();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder();
            var command = CreateCommand(sourceModel, settings);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.GenericTypeArguments.ShouldBeEmpty();
            response.GenericTypeArgumentConstraints.ShouldBeEmpty();
            response.Abstract.ShouldBeFalse();
        }

        [Fact]
        public async Task Returns_Error_When_Parsing_NameFormatString_Is_Not_Successful()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").Build();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(enableEntityInheritance: true, builderNameFormatString: "{Error}");
            var command = CreateCommand(sourceModel, settings);

            // Act
            var result = await sut.ExecuteAsync(command, new ClassBuilder(), CommandService, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Error);
            result.ErrorMessage.ShouldBe("Kaboom");
        }

        private static GenerateBuilderCommand CreateCommand(TypeBase sourceModel, PipelineSettingsBuilder settings)
            => new GenerateBuilderCommand(sourceModel, settings, CultureInfo.InvariantCulture);
    }
}

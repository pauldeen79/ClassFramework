﻿namespace ClassFramework.Pipelines.Tests.Builder.Components;

public class AbstractBuilderComponentTests : TestBase<Pipelines.Builder.Components.AbstractBuilderComponent>
{
    public class ExecuteAsync : AbstractBuilderComponentTests
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
        public async Task Adds_AddGenericTypeArguments_When_IsBuilderForAbstractEntity_Is_True()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").Build();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(enableEntityInheritance: true);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ExecuteAsync(context, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Builder.GenericTypeArguments.ToArray().ShouldBeEquivalentTo(new[] { "TBuilder", "TEntity" });
            context.Builder.GenericTypeArgumentConstraints.ToArray().ShouldBeEquivalentTo(new[] { "where TEntity : SomeNamespace.SomeClass", "where TBuilder : SomeClassBuilder<TBuilder, TEntity>" });
            context.Builder.Abstract.ShouldBeTrue();
        }

        [Fact]
        public async Task Does_Not_Add_AddGenericTypeArguments_When_IsBuilderForAbstractEntity_Is_False_And_Validation_Is_Not_Shared_Between_Builder_And_Entity()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").Build();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder();
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ExecuteAsync(context, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Builder.GenericTypeArguments.ShouldBeEmpty();
            context.Builder.GenericTypeArgumentConstraints.ShouldBeEmpty();
            context.Builder.Abstract.ShouldBeFalse();
        }

        [Fact]
        public async Task Returns_Error_When_Parsing_NameFormatString_Is_Not_Successful()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").Build();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(enableEntityInheritance: true, builderNameFormatString: "{Error}");
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ExecuteAsync(context, CommandService, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Error);
            result.ErrorMessage.ShouldBe("Kaboom");
        }

        private static BuilderContext CreateContext(TypeBase sourceModel, PipelineSettingsBuilder settings)
            => new BuilderContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None);
    }
}

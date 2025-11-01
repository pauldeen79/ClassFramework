﻿namespace ClassFramework.Pipelines.Tests.Builder.Components;

public class AddBuildMethodComponentTests : TestBase<Pipelines.Builder.Components.AddBuildMethodComponent>
{
    public class ExecuteAsync : AddBuildMethodComponentTests
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
        public async Task Adds_Build_Method_When_EnableBuilderInheritance_And_IsAbstract_Are_Both_True()
        {
            // Arrange
            var sourceModel = CreateClass();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(enableBuilderInheritance: true, isAbstract: true);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ExecuteAsync(context, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Builder.Methods.Count.ShouldBe(2);
            context.Builder.Methods.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "Build", "BuildTyped" });
        }

        [Fact]
        public async Task Adds_Build_Method_When_IsBuilderForAbstractEntity_Is_False()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder();
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ExecuteAsync(context, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Builder.Methods.Count.ShouldBe(1);
            var method = context.Builder.Methods.Single();
            method.Name.ShouldBe("Build");
            method.CodeStatements.ShouldAllBe(x => x is StringCodeStatementBuilder);
            method.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "return new SomeNamespace.SomeClass { Property1 = Property1, Property2 = Property2, Property3 = Property3 };"
                }
            );
        }

        [Fact]
        public async Task Adds_Build_And_BuildTyped_Methods_When_IsBuilderForAbstractEntity_Is_True()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(enableEntityInheritance: true);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ExecuteAsync(context, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Builder.Methods.Count.ShouldBe(2);

            var buildMethod = context.Builder.Methods.SingleOrDefault(x => x.Name == "Build");
            buildMethod.ShouldNotBeNull(customMessage: "Build method should exist");
            buildMethod!.Abstract.ShouldBeFalse();
            buildMethod.ReturnTypeName.ShouldBe("SomeNamespace.SomeClass");
            buildMethod.CodeStatements.ShouldAllBe(x => x is StringCodeStatementBuilder);
            buildMethod.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "return BuildTyped();"
                }
            );

            var buildTypedMethod = context.Builder.Methods.SingleOrDefault(x => x.Name == "BuildTyped");
            buildTypedMethod.ShouldNotBeNull(customMessage: "BuildTyped method should exist");
            buildTypedMethod!.Abstract.ShouldBeTrue();
            buildTypedMethod.ReturnTypeName.ShouldBe("TEntity");
            buildTypedMethod.CodeStatements.ShouldBeEmpty();
        }

        [Fact]
        public async Task Returns_Error_When_Parsing_EntityInstanciation_Is_Not_Successful()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(typenameMappings:
            [
                new TypenameMappingBuilder()
                    .WithSourceType(typeof(int))
                    .WithTargetType(typeof(int))
                    .AddMetadata(new MetadataBuilder().WithName(MetadataNames.CustomBuilderMethodParameterExpression).WithValue("{Error}"))
            ]);
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

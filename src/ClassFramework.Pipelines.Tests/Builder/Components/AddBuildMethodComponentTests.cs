namespace ClassFramework.Pipelines.Tests.Builder.Components;

public class AddBuildMethodComponentTests : TestBase<Pipelines.Builder.Components.AddBuildMethodComponent>
{
    public class ProcessAsync : AddBuildMethodComponentTests
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
        public async Task Adds_Build_Method_When_EnableBuilderInheritance_And_IsAbstract_Are_Both_True()
        {
            // Arrange
            var sourceModel = CreateClass();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(enableBuilderInheritance: true, isAbstract: true);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Methods.Should().HaveCount(2);
            context.Request.Builder.Methods.Select(x => x.Name).Should().BeEquivalentTo("Build", "BuildTyped");
        }

        [Fact]
        public async Task Adds_Build_Method_When_IsBuilderForAbstractEntity_Is_False()
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder();
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Methods.Should().ContainSingle();
            var method = context.Request.Builder.Methods.Single();
            method.Name.Should().Be("Build");
            method.CodeStatements.Should().AllBeOfType<StringCodeStatementBuilder>();
            method.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo
            (
                "return new SomeNamespace.SomeClass { Property1 = Property1, Property2 = Property2, Property3 = Property3 };"
            );
        }

        [Fact]
        public async Task Adds_Build_And_BuildTyped_Methods_When_IsBuilderForAbstractEntity_Is_True()
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(enableEntityInheritance: true);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Methods.Should().HaveCount(2);

            var buildMethod = context.Request.Builder.Methods.SingleOrDefault(x => x.Name == "Build");
            buildMethod.Should().NotBeNull(because: "Build method should exist");
            buildMethod!.Abstract.Should().BeFalse();
            buildMethod.ReturnTypeName.Should().Be("SomeNamespace.SomeClass");
            buildMethod.CodeStatements.Should().AllBeOfType<StringCodeStatementBuilder>();
            buildMethod.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).Should().BeEquivalentTo
            (
                "return BuildTyped();"
            );

            var buildTypedMethod = context.Request.Builder.Methods.SingleOrDefault(x => x.Name == "BuildTyped");
            buildTypedMethod.Should().NotBeNull(because: "BuildTyped method should exist");
            buildTypedMethod!.Abstract.Should().BeTrue();
            buildTypedMethod.ReturnTypeName.Should().Be("TEntity");
            buildTypedMethod.CodeStatements.Should().BeEmpty();
        }

        [Fact]
        public async Task Returns_Error_When_Parsing_EntityInstanciation_Is_Not_Successful()
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
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
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Error);
            result.ErrorMessage.Should().Be("Kaboom");
        }

        private static PipelineContext<BuilderContext> CreateContext(TypeBase sourceModel, PipelineSettingsBuilder settings)
            => new(new BuilderContext(sourceModel, settings, CultureInfo.InvariantCulture));
    }
}

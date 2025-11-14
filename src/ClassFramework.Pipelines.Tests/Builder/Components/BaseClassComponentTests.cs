namespace ClassFramework.Pipelines.Tests.Builder.Components;

public class BaseClassComponentTests : TestBase<Pipelines.Builder.Components.BaseClassComponent>
{
    public class ExecuteAsync : BaseClassComponentTests
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
        public async Task Sets_BaseClass_For_BuilderInheritance_And_Not_For_Abstract_Builder_No_BaseClass()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                enableBuilderInheritance: true,
                baseClass: null,
                enableEntityInheritance: true);
            var command = CreateCommand(sourceModel, settings);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.BaseClass.ShouldBe("SomeClassBuilder");
        }

        [Fact]
        public async Task Sets_BaseClass_For_BuilderInheritance_And_Not_For_Abstract_Builder_With_BaseClass_And_Abstract()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                enableBuilderInheritance: true,
                baseClass: new ClassBuilder().WithName("BaseClass").BuildTyped(),
                isAbstract: true,
                enableEntityInheritance: true);
            var command = CreateCommand(sourceModel, settings);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.BaseClass.ShouldBe("SomeClassBuilder");
        }

        [Fact]
        public async Task Sets_BaseClass_For_BuilderInheritance_And_Not_For_Abstract_Builder_With_BaseClass_And_Not_Abstract_No_Builders_Namespace()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                enableBuilderInheritance: true,
                baseClass: new ClassBuilder().WithName("BaseClass").BuildTyped(),
                isAbstract: false,
                enableEntityInheritance: true);
            var command = CreateCommand(sourceModel, settings);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.BaseClass.ShouldBe("BaseClassBuilder<SomeClassBuilder, SomeNamespace.SomeClass>");
        }

        [Fact]
        public async Task Sets_BaseClass_For_BuilderInheritance_And_Not_For_Abstract_Builder_With_BaseClass_And_Not_Abstract_Builders_Namespace()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                enableBuilderInheritance: true,
                baseClass: new ClassBuilder().WithName("BaseClass").BuildTyped(),
                isAbstract: false,
                baseClassBuilderNameSpace: "BaseBuilders",
                enableEntityInheritance: true);
            var command = CreateCommand(sourceModel, settings);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.BaseClass.ShouldBe("BaseBuilders.BaseClassBuilder<SomeClassBuilder, SomeNamespace.SomeClass>");
        }

        [Fact]
        public async Task Sets_BaseClass_For_BuilderInheritance_For_Abstract_Builder_With_BaseClass()
        {
            // Arrange
            var sourceModel = CreateClass("MyBaseClass");
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                enableBuilderInheritance: true,
                enableEntityInheritance: true).WithIsForAbstractBuilder();
            var command = CreateCommand(sourceModel, settings);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.BaseClass.ShouldBe("MyBaseClassBuilder");
        }

        [Fact]
        public async Task Sets_BaseClass_For_No_BuilderInheritance_For_Abstract_Builder_With_BaseClass()
        {
            // Arrange
            var sourceModel = CreateClass("MyBaseClass");
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                enableBuilderInheritance: false,
                enableEntityInheritance: true).WithIsForAbstractBuilder();
            var command = CreateCommand(sourceModel, settings);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.BaseClass.ShouldBe("MyBaseClassBuilder<SomeClassBuilder, SomeNamespace.SomeClass>");
        }

        [Fact]
        public async Task Sets_BaseClass_With_Custom_Value_When_Metadata_Is_Available()
        {
            // Arrange
            var sourceModel = CreateClass("MyBaseClass");
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                enableBuilderInheritance: false,
                enableEntityInheritance: true)
                .WithIsForAbstractBuilder()
                .AddTypenameMappings(new TypenameMappingBuilder("MyBaseClass")
                    .AddMetadata(MetadataNames.CustomBuilderBaseClassTypeName, "xyz.CustomBaseClassBuilder"));
            var command = CreateCommand(sourceModel, settings);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.BaseClass.ShouldBe("xyz.CustomBaseClassBuilder<SomeClassBuilder, SomeNamespace.SomeClass>");
        }

        [Fact]
        public async Task Returns_Error_When_Parsing_BuilderNameFormatString_Is_Not_Succesful()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                enableBuilderInheritance: true,
                baseClass: null,
                enableEntityInheritance: true,
                builderNameFormatString: "{Error}");
            var command = CreateCommand(sourceModel, settings);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Error);
            result.ErrorMessage.ShouldBe("Kaboom");
        }

        private static GenerateBuilderCommand CreateCommand(TypeBase sourceModel, PipelineSettingsBuilder settings)
            => new GenerateBuilderCommand(sourceModel, settings, CultureInfo.InvariantCulture);
    }
}

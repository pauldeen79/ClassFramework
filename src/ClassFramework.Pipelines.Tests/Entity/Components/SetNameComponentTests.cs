namespace ClassFramework.Pipelines.Tests.Entity.Components;

public class SetNameComponentTests : TestBase<Pipelines.Entity.Components.SetNameComponent>
{
    public class ExecuteAsync : SetNameComponentTests
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
        public async Task Sets_Name_Property_With_Default_FormatString()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity();
            var command = new GenerateEntityCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Name.ShouldBe("SomeClass");
        }

        [Fact]
        public async Task Sets_Name_Property_With_Custom_FormatString()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(entityNameFormatString: "CustomClassName");
            var command = new GenerateEntityCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Name.ShouldBe("CustomClassName");
        }

        [Fact]
        public async Task Sets_Namespace_Property_With_Default_FormatString()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity();
            var command = new GenerateEntityCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Namespace.ShouldBe("SomeNamespace");
        }

        [Fact]
        public async Task Sets_Namespace_Property_With_Custom_FormatString()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(entityNamespaceFormatString: "CustomNamespace");
            var command = new GenerateEntityCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Namespace.ShouldBe("CustomNamespace");
        }

        [Fact]
        public async Task Returns_Error_When_Parsing_NameFormatString_Is_Not_Successful()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(entityNameFormatString: "{Error}");
            var command = new GenerateEntityCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Error);
            result.ErrorMessage.ShouldBe("Kaboom");
        }

        [Fact]
        public async Task Returns_Error_When_Parsing_NamespaceFormatString_Is_Not_Successful()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(entityNamespaceFormatString: "{Error}");
            var command = new GenerateEntityCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Error);
            result.ErrorMessage.ShouldBe("Kaboom");
        }
    }
}

namespace ClassFramework.Pipelines.Tests.Entity.Components;

public class SetNameComponentTests : TestBase<Pipelines.Entity.Components.SetNameComponent>
{
    public class ExecuteAsync : SetNameComponentTests
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
        public async Task Sets_Name_Property_With_Default_FormatString()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity();
            var context = new EntityContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None);

            // Act
            var result = await sut.ExecuteAsync(context, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Builder.Name.ShouldBe("SomeClass");
        }

        [Fact]
        public async Task Sets_Name_Property_With_Custom_FormatString()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(entityNameFormatString: "CustomClassName");
            var context = new EntityContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None);

            // Act
            var result = await sut.ExecuteAsync(context, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Builder.Name.ShouldBe("CustomClassName");
        }

        [Fact]
        public async Task Sets_Namespace_Property_With_Default_FormatString()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity();
            var context = new EntityContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None);

            // Act
            var result = await sut.ExecuteAsync(context, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Builder.Namespace.ShouldBe("SomeNamespace");
        }

        [Fact]
        public async Task Sets_Namespace_Property_With_Custom_FormatString()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(entityNamespaceFormatString: "CustomNamespace");
            var context = new EntityContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None);

            // Act
            var result = await sut.ExecuteAsync(context, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Builder.Namespace.ShouldBe("CustomNamespace");
        }

        [Fact]
        public async Task Returns_Error_When_Parsing_NameFormatString_Is_Not_Successful()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(entityNameFormatString: "{Error}");
            var context = new EntityContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None);

            // Act
            var result = await sut.ExecuteAsync(context, CommandService, CancellationToken.None);

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
            var context = new EntityContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None);

            // Act
            var result = await sut.ExecuteAsync(context, CommandService, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Error);
            result.ErrorMessage.ShouldBe("Kaboom");
        }
    }
}

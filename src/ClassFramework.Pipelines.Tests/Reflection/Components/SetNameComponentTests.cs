namespace ClassFramework.Pipelines.Tests.Reflection.Components;

public class SetNameComponentTests : TestBase<Pipelines.Reflection.Components.SetNameComponent>
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
        public async Task Sets_Name_Property()
        {
            // Arrange
            var sourceModel = typeof(MyClass);
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForReflection();
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ExecuteAsync(context, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Builder.Name.ShouldBe("MyClass");
        }

        [Fact]
        public async Task Sets_Namespace_Property()
        {
            // Arrange
            var sourceModel = typeof(MyClass);
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForReflection();
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ExecuteAsync(context, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Builder.Namespace.ShouldBe("ClassFramework.Pipelines.Tests.Reflection");
        }

        [Fact]
        public async Task Returns_Error_When_Parsing_BuilderNameFormatString_Is_Not_Succesful()
        {
            // Arrange
            var sourceModel = typeof(MyClass);
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForReflection(nameFormatString: "{Error}");
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ExecuteAsync(context, CommandService, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Error);
            result.ErrorMessage.ShouldBe("Kaboom");
        }

        [Fact]
        public async Task Returns_Error_When_Parsing_BuilderNameSpaceFormatString_Is_Not_Succesful()
        {
            // Arrange
            var sourceModel = typeof(MyClass);
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForReflection(namespaceFormatString: "{Error}");
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ExecuteAsync(context, CommandService, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(ResultStatus.Error);
            result.ErrorMessage.ShouldBe("Kaboom");
        }

        private static ReflectionContext CreateContext(Type sourceModel, PipelineSettingsBuilder settings)
            => new ReflectionContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None);
    }
}

namespace ClassFramework.Pipelines.Tests.Reflection.Components;

public class SetNameComponentTests : TestBase<Pipelines.Reflection.Components.SetNameComponent>
{
    public class ProcessAsync : SetNameComponentTests
    {
        [Fact]
        public async Task Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            var t = sut.ProcessAsync(context: null!);
            (await Should.ThrowAsync<ArgumentNullException>(t))
             .ParamName.ShouldBe("context");
        }

        [Fact]
        public async Task Sets_Name_Property()
        {
            // Arrange
            var sourceModel = typeof(MyClass);
            await InitializeExpressionEvaluator();
            var sut = CreateSut();
            var settings = CreateSettingsForReflection();
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Name.ShouldBe("MyClass");
        }

        [Fact]
        public async Task Sets_Namespace_Property()
        {
            // Arrange
            var sourceModel = typeof(MyClass);
            await InitializeExpressionEvaluator();
            var sut = CreateSut();
            var settings = CreateSettingsForReflection();
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Namespace.ShouldBe("ClassFramework.Pipelines.Tests.Reflection");
        }

        [Fact]
        public async Task Returns_Error_When_Parsing_BuilderNameFormatString_Is_Not_Succesful()
        {
            // Arrange
            var sourceModel = typeof(MyClass);
            await InitializeExpressionEvaluator();
            var sut = CreateSut();
            var settings = CreateSettingsForReflection(nameFormatString: "{Error}");
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Error);
            result.ErrorMessage.ShouldBe("Kaboom");
        }

        [Fact]
        public async Task Returns_Error_When_Parsing_BuilderNameSpaceFormatString_Is_Not_Succesful()
        {
            // Arrange
            var sourceModel = typeof(MyClass);
            await InitializeExpressionEvaluator();
            var sut = CreateSut();
            var settings = CreateSettingsForReflection(namespaceFormatString: "{Error}");
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Error);
            result.ErrorMessage.ShouldBe("Kaboom");
        }

        private static PipelineContext<ReflectionContext> CreateContext(Type sourceModel, PipelineSettingsBuilder settings)
            => new(new ReflectionContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None));
    }
}

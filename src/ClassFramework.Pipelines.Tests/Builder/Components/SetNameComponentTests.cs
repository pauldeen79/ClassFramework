namespace ClassFramework.Pipelines.Tests.Builder.Components;

public class SetNameComponentTests : TestBase<Pipelines.Builder.Components.SetNameComponent>
{
    public class Process : SetNameComponentTests
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
        public async Task Sets_Name_Property()
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
            context.Request.Builder.Name.Should().Be("SomeClassBuilder");
        }

        [Fact]
        public async Task Sets_Namespace_Property()
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
            context.Request.Builder.Namespace.Should().Be("SomeNamespace.Builders");
        }

        [Fact]
        public async Task Returns_Error_When_Parsing_BuilderNameFormatString_Is_Not_Succesful()
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(builderNameFormatString: "{Error}");
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Error);
            result.ErrorMessage.Should().Be("Kaboom");
        }

        [Fact]
        public async Task Returns_Error_When_Parsing_BuilderNameSpaceFormatString_Is_Not_Succesful()
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(builderNamespaceFormatString: "{Error}");
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Error);
            result.ErrorMessage.Should().Be("Kaboom");
        }

        private static PipelineContext<BuilderContext> CreateContext(TypeBase sourceModel, PipelineSettingsBuilder settings)
            => new(new BuilderContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));
    }
}

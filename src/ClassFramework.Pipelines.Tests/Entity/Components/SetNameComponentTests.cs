namespace ClassFramework.Pipelines.Tests.Entity.Components;

public class SetNameComponentTests : TestBase<Pipelines.Entity.Components.SetNameComponent>
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
        public async Task Sets_Name_Property_With_Default_FormatString()
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity();
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Name.Should().Be("SomeClass");
        }

        [Fact]
        public async Task Sets_Name_Property_With_Custom_FormatString()
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(entityNameFormatString: "CustomClassName");
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Name.Should().Be("CustomClassName");
        }

        [Fact]
        public async Task Sets_Namespace_Property_With_Default_FormatString()
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity();
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Namespace.Should().Be("SomeNamespace");
        }

        [Fact]
        public async Task Sets_Namespace_Property_With_Custom_FormatString()
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(entityNamespaceFormatString: "CustomNamespace");
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Namespace.Should().Be("CustomNamespace");
        }

        [Fact]
        public async Task Returns_Error_When_Parsing_NameFormatString_Is_Not_Successful()
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(entityNameFormatString: "{Error}");
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Error);
            result.ErrorMessage.Should().Be("Kaboom");
        }

        [Fact]
        public async Task Returns_Error_When_Parsing_NamespaceFormatString_Is_Not_Successful()
        {
            // Arrange
            var sourceModel = CreateClass();
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(entityNamespaceFormatString: "{Error}");
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Error);
            result.ErrorMessage.Should().Be("Kaboom");
        }
    }
}

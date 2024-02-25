namespace ClassFramework.Pipelines.Tests.Builder.Features;

public class AddMetadataFeatureTests : TestBase<Pipelines.Builder.Features.AddMetadataFeature>
{
    public class Process : AddMetadataFeatureTests
    {
        [Fact]
        public void Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            sut.Invoking(x => x.Process(context: null!))
               .Should().Throw<ArgumentNullException>().WithParameterName("context");
        }

        [Fact]
        public void Adds_Metadata_From_SourceModel()
        {
            // Arrange
            var sourceModel = CreateModel();
            InitializeParser();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForBuilder().Build();
            var context = new PipelineContext<IConcreteTypeBuilder, BuilderContext>(model, new BuilderContext(sourceModel, settings, CultureInfo.InvariantCulture));

            // Act
            var result = sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.Metadata.Select(x => x.Name).Should().BeEquivalentTo("MyMetadataName");
            model.Metadata.Select(x => x.Value).Should().BeEquivalentTo(new object[] { "MyMetadataValue" });
        }
    }
}

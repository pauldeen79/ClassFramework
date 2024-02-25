namespace ClassFramework.Pipelines.Tests.Shared.Features;

public class PartialFeatureTests : TestBase<PartialFeature>
{
    public class Process : PartialFeatureTests
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
        public void Sets_Partial_Property_On_Model_To_True()
        {
            // Arrange
            var sut = CreateSut();
            var model = new ClassBuilder();
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").Build();
            var context = new PipelineContext<IConcreteTypeBuilder, BuilderContext>(model, new BuilderContext(sourceModel, new PipelineSettingsBuilder().Build(), CultureInfo.InvariantCulture));

            // Act
            sut.Process(context);

            // Assert
            model.Partial.Should().BeTrue();
        }
    }
}

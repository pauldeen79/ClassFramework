namespace ClassFramework.Pipelines.Tests.Reflection.Features;

public class AddMethodsComponentTests : TestBase<Pipelines.Reflection.Features.AddMethodsComponent>
{
    public class Process : AddMethodsComponentTests
    {
        [Fact]
        public void Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            sut.Awaiting(x => x.Process(context: null!))
               .Should().ThrowAsync<ArgumentNullException>().WithParameterName("context");
        }

        [Fact]
        public async Task Copies_Methods()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyClass);
            var model = new ClassBuilder();
            var settings = CreateSettingsForReflection();
            var context = new PipelineContext<ReflectionContext, TypeBaseBuilder>(new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture), model);

            // Act
            var result = await sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.Methods.Should().ContainSingle();
        }
    }
}

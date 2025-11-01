namespace ClassFramework.Pipelines.Tests;

public class ParentChildContextTests : TestBase
{
    public class Constructor : ParentChildContextTests
    {
        [Fact]
        public void Throws_On_Null_ParentContext()
        {
            // Arrange
            var parentContext = default(BuilderContext);
            var childContext = new PropertyBuilder().WithName("Property").WithType(typeof(int)).Build();
            var settings = new PipelineSettingsBuilder();

            // Act & Assert
            Action a = () => _ = new ParentChildContext<BuilderContext, Property>(parentContext!, childContext, settings);
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("parentContext");
        }

        [Fact]
        public void Throws_On_Null_ChildContext()
        {
            // Arrange
            var parentContext = new BuilderContext(new ClassBuilder().WithName("MyClass").Build(), new PipelineSettingsBuilder(), CultureInfo.CurrentCulture, CancellationToken.None);
            var childContext = default(Property);
            var settings = new PipelineSettingsBuilder();

            // Act & Assert
            Action a = () => _ = new ParentChildContext<BuilderContext, Property>(parentContext, childContext!, settings);
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("childContext");
        }

        [Fact]
        public void Throws_On_Null_Settings()
        {
            // Arrange
            var parentContext = new BuilderContext(new ClassBuilder().WithName("MyClass").Build(), new PipelineSettingsBuilder(), CultureInfo.CurrentCulture, CancellationToken.None);
            var childContext = new PropertyBuilder().WithName("Property").WithType(typeof(int)).Build();
            var settings = default(PipelineSettings);

            // Act & Assert
            Action a = () => _ = new ParentChildContext<BuilderContext, Property>(parentContext, childContext, settings!);
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("settings");
        }
    }
}

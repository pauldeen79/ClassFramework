namespace ClassFramework.Pipelines.Tests;

public class ParentChildContextTests : TestBase
{
    public class Constructor : ParentChildContextTests
    {
        [Fact]
        public void Throws_On_Null_ParentContext()
        {
            // Arrange
            var parentContext = default(PipelineContext<ClassBuilder, TypeBase>);
            var childContext = new PropertyBuilder().WithName("Property").WithType(typeof(int)).Build();
            var settings = new PipelineSettingsBuilder();

            // Act & Assert
            Action a = () => _ = new ParentChildContext<PipelineContext<ClassBuilder, TypeBase>, Property>(parentContext!, childContext, settings);
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("parentContext");
        }

        [Fact]
        public void Throws_On_Null_ChildContext()
        {
            // Arrange
            var parentContext = new PipelineContext<ClassBuilder, TypeBase>(new ClassBuilder(), new ClassBuilder().WithName("MyClass").Build());
            var childContext = default(Property);
            var settings = new PipelineSettingsBuilder();

            // Act & Assert
            Action a = () => _ = new ParentChildContext<PipelineContext<ClassBuilder, TypeBase>, Property>(parentContext, childContext!, settings);
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("childContext");
        }

        [Fact]
        public void Throws_On_Null_Settings()
        {
            // Arrange
            var parentContext = new PipelineContext<ClassBuilder, TypeBase>(new ClassBuilder(), new ClassBuilder().WithName("MyClass").Build());
            var childContext = new PropertyBuilder().WithName("Property").WithType(typeof(int)).Build();
            var settings = default(PipelineSettings);

            // Act & Assert
            Action a = () => _ = new ParentChildContext<PipelineContext<ClassBuilder, TypeBase>, Property>(parentContext, childContext, settings!);
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("settings");
        }
    }
}

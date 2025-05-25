namespace ClassFramework.Pipelines.Tests.Reflection;

public class ReflectionContextTests : TestBase
{
    public class Constructor : ReflectionContextTests
    {
        [Fact]
        public void Throws_On_Null_SourceModel()
        {
            // Act & Assert
            Action a = () => _ = new ReflectionContext(sourceModel: null!, new PipelineSettingsBuilder(), CultureInfo.InvariantCulture);
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("sourceModel");
        }

        [Fact]
        public void Throws_On_Null_Settings()
        {
            // Act & Assert
            Action a = () => _ = new ReflectionContext(sourceModel: GetType(), settings: null!, CultureInfo.InvariantCulture);
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("settings");
        }

        [Fact]
        public void Throws_On_Null_FormatProvider()
        {
            // Act & Assert
            Action a = () => _ = new ReflectionContext(sourceModel: GetType(), new PipelineSettingsBuilder(), formatProvider: null!);
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("formatProvider");
        }
    }

    public class MapTypeName : ReflectionContextTests
    {
        [Fact]
        public void Throws_On_Null_TypeName()
        {
            // Arrange
            var settings = CreateSettingsForBuilder(enableNullableReferenceTypes: false);
            var sut = new BuilderContext(CreateClass(), settings, CultureInfo.InvariantCulture);

            // Act & Assert
            Action a = () => sut.MapTypeName(typeName: null!, string.Empty);
            a.ShouldThrow<ArgumentNullException>()
             .ParamName.ShouldBe("typeName");
        }
    }

    public class MapAttribute : ReflectionContextTests
    {
        [Fact]
        public void Throws_On_Null_TypeName()
        {
            // Arrange
            var settings = CreateSettingsForReflection();
            var sut = new ReflectionContext(GetType(), settings, CultureInfo.InvariantCulture);

            // Act & Assert
            Action a = () => sut.MapAttribute(attribute: null!);
            a.ShouldThrow<ArgumentNullException>()
             .ParamName.ShouldBe("attribute");
        }
    }
}

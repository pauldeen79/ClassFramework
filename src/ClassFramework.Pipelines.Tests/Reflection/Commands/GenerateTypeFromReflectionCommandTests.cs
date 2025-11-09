namespace ClassFramework.Pipelines.Tests.Reflection.Commands;

public class GenerateTypeFromReflectionCommandTests : TestBase
{
    public class Constructor : GenerateTypeFromReflectionCommandTests
    {
        [Fact]
        public void Throws_On_Null_SourceModel()
        {
            // Act & Assert
            Action a = () => _ = new GenerateTypeFromReflectionCommand(sourceModel: null!, new PipelineSettingsBuilder(), CultureInfo.InvariantCulture);
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("sourceModel");
        }

        [Fact]
        public void Throws_On_Null_Settings()
        {
            // Act & Assert
            Action a = () => _ = new GenerateTypeFromReflectionCommand(sourceModel: GetType(), settings: null!, CultureInfo.InvariantCulture);
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("settings");
        }

        [Fact]
        public void Throws_On_Null_FormatProvider()
        {
            // Act & Assert
            Action a = () => _ = new GenerateTypeFromReflectionCommand(sourceModel: GetType(), new PipelineSettingsBuilder(), formatProvider: null!);
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("formatProvider");
        }
    }

    public class MapTypeName : GenerateTypeFromReflectionCommandTests
    {
        [Fact]
        public void Throws_On_Null_TypeName()
        {
            // Arrange
            var settings = CreateSettingsForBuilder(enableNullableReferenceTypes: false);
            var sut = new GenerateBuilderCommand(CreateClass(), settings, CultureInfo.InvariantCulture);

            // Act & Assert
            Action a = () => sut.MapTypeName(typeName: null!);
            a.ShouldThrow<ArgumentNullException>()
             .ParamName.ShouldBe("typeName");
        }
    }

    public class MapAttribute : GenerateTypeFromReflectionCommandTests
    {
        [Fact]
        public void Throws_On_Null_TypeName()
        {
            // Arrange
            var settings = CreateSettingsForReflection();
            var sut = new GenerateTypeFromReflectionCommand(GetType(), settings, CultureInfo.InvariantCulture);

            // Act & Assert
            Action a = () => sut.MapAttribute(attribute: null!);
            a.ShouldThrow<ArgumentNullException>()
             .ParamName.ShouldBe("attribute");
        }
    }
}

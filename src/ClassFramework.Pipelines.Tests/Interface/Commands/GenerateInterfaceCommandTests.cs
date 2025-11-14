namespace ClassFramework.Pipelines.Tests.Interface.Commands;

public class GenerateInterfaceCommandTests : TestBase
{
    public class Constructor : GenerateInterfaceCommandTests
    {
        [Fact]
        public void Throws_On_Null_SourceModel()
        {
            // Act & Assert
            Action a = () => _ = new GenerateInterfaceCommand(sourceModel: null!, new PipelineSettingsBuilder(), CultureInfo.InvariantCulture);
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("sourceModel");
        }

        [Fact]
        public void Throws_On_Null_Settings()
        {
            // Act & Assert
            Action a = () => _ = new GenerateInterfaceCommand(sourceModel: CreateClass(), settings: null!, CultureInfo.InvariantCulture);
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("settings");
        }

        [Fact]
        public void Throws_On_Null_FormatProvider()
        {
            // Act & Assert
            Action a = () => _ = new GenerateInterfaceCommand(sourceModel: CreateClass(), new PipelineSettingsBuilder(), formatProvider: null!);
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("formatProvider");
        }
    }

    public class MapTypeName : GenerateInterfaceCommandTests
    {
        [Fact]
        public void Throws_On_Null_TypeName()
        {
            // Arrange
            var settings = CreateSettingsForBuilder(enableNullableReferenceTypes: false).Build();
            var sut = new GenerateBuilderCommand(CreateClass(), settings, CultureInfo.InvariantCulture);

            // Act & Assert
            Action a = () => sut.MapTypeName(typeName: null!);
            a.ShouldThrow<ArgumentNullException>()
             .ParamName.ShouldBe("typeName");
        }
    }

    public class MapAttribute : GenerateInterfaceCommandTests
    {
        [Fact]
        public void Throws_On_Null_TypeName()
        {
            // Arrange
            var settings = CreateSettingsForBuilder(enableNullableReferenceTypes: false).Build();
            var sut = new GenerateBuilderCommand(CreateClass(), settings, CultureInfo.InvariantCulture);

            // Act & Assert
            Action a = () => sut.MapAttribute(attribute: null!);
            a.ShouldThrow<ArgumentNullException>()
             .ParamName.ShouldBe("attribute");
        }
    }
}

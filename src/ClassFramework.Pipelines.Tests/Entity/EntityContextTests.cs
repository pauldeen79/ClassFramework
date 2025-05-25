namespace ClassFramework.Pipelines.Tests.Entity;

public class EntityContextTests : TestBase
{
    public class Constructor : EntityContextTests
    {
        [Fact]
        public void Throws_On_Null_SourceModel()
        {
            // Act & Assert
            Action a = () => _ = new EntityContext(sourceModel: null!, new PipelineSettingsBuilder(), CultureInfo.InvariantCulture);
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("sourceModel");
        }

        [Fact]
        public void Throws_On_Null_Settings()
        {
            // Act & Assert
            Action a = () => _ = new EntityContext(sourceModel: CreateClass(), settings: null!, CultureInfo.InvariantCulture);
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("settings");
        }

        [Fact]
        public void Throws_On_Null_FormatProvider()
        {
            // Act & Assert
            Action a = () => _ = new EntityContext(sourceModel: CreateClass(), new PipelineSettingsBuilder(), formatProvider: null!);
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("formatProvider");
        }
    }

    public class MapTypeName : EntityContextTests
    {
        [Fact]
        public void Throws_On_Null_TypeName()
        {
            // Arrange
            var settings = CreateSettingsForBuilder(enableNullableReferenceTypes: false);
            var sut = new EntityContext(CreateClass(), settings, CultureInfo.InvariantCulture);

            // Act & Assert
            Action a = () => sut.MapTypeName(typeName: null!, string.Empty);
            a.ShouldThrow<ArgumentNullException>()
             .ParamName.ShouldBe("typeName");
        }
    }

    public class MapAttribute : EntityContextTests
    {
        [Fact]
        public void Throws_On_Null_TypeName()
        {
            // Arrange
            var settings = CreateSettingsForBuilder(enableNullableReferenceTypes: false);
            var sut = new EntityContext(CreateClass(), settings, CultureInfo.InvariantCulture);

            // Act & Assert
            Action a = () => sut.MapAttribute(attribute: null!);
            a.ShouldThrow<ArgumentNullException>()
             .ParamName.ShouldBe("attribute");
        }
    }
}

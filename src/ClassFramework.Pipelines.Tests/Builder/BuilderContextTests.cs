namespace ClassFramework.Pipelines.Tests.Builder;

public class BuilderContextTests : TestBase
{
    public class Constructor : BuilderContextTests
    {
        [Fact]
        public void Throws_On_Null_SourceModel()
        {
            // Act & Assert
            Action a = () => _ = new BuilderContext(sourceModel: null!, new PipelineSettingsBuilder(), CultureInfo.InvariantCulture);
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("sourceModel");
        }

        [Fact]
        public void Throws_On_Null_Settings()
        {
            // Act & Assert
            Action a = () => _ = new BuilderContext(sourceModel: CreateClass(), settings: null!, CultureInfo.InvariantCulture);
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("settings");
        }

        [Fact]
        public void Throws_On_Null_FormatProvider()
        {
            // Act & Assert
            Action a = () => _ = new BuilderContext(sourceModel: CreateClass(), new PipelineSettingsBuilder(), formatProvider: null!);
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("formatProvider");
        }
    }

    public class CreatePragmaWarningDisableStatements : BuilderContextTests
    {
        [Fact]
        public void Returns_Empty_Array_When_Pragmas_Are_Not_Needed()
        {
            // Arrange
            var settings = CreateSettingsForBuilder(enableNullableReferenceTypes: false);
            var sut = new BuilderContext(CreateClass(), settings, CultureInfo.InvariantCulture);

            // Act
            var result = sut.CreatePragmaWarningDisableStatementsForBuildMethod();

            // Assert
            result.ShouldBeEmpty();
        }

        [Fact]
        public void Returns_Correct_Result_When_Pragmas_Are_Needed()
        {
            // Arrange
            var settings = CreateSettingsForBuilder(enableNullableReferenceTypes: true);
            var sut = new BuilderContext(CreateClass(), settings, CultureInfo.InvariantCulture);

            // Act
            var result = sut.CreatePragmaWarningDisableStatementsForBuildMethod();

            // Assert
            result.ShouldBeEquivalentTo
            (
                new[]
                {
                    "#pragma warning disable CS8604 // Possible null reference argument.",
                    "#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type."
                }
            );
        }
    }

    public class CreatePragmaWarningRestoreStatements : BuilderContextTests
    {
        [Fact]
        public void Returns_Empty_Array_When_Pragmas_Are_Not_Needed()
        {
            // Arrange
            var settings = CreateSettingsForBuilder(enableNullableReferenceTypes: false);
            var sut = new BuilderContext(CreateClass(), settings, CultureInfo.InvariantCulture);

            // Act
            var result = sut.CreatePragmaWarningRestoreStatementsForBuildMethod();

            // Assert
            result.ShouldBeEmpty();
        }

        [Fact]
        public void Returns_Correct_Result_When_Pragmas_Are_Needed()
        {
            // Arrange
            var settings = CreateSettingsForBuilder(enableNullableReferenceTypes: true);
            var sut = new BuilderContext(CreateClass(), settings, CultureInfo.InvariantCulture);

            // Act
            var result = sut.CreatePragmaWarningRestoreStatementsForBuildMethod();

            // Assert
            result.ShouldBeEquivalentTo
            (
                new[]
                {
                    "#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.",
                    "#pragma warning restore CS8604 // Possible null reference argument."
                }
            );
        }
    }

    public class MapTypeName : BuilderContextTests
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

    public class MapAttribute : BuilderContextTests
    {
        [Fact]
        public void Throws_On_Null_TypeName()
        {
            // Arrange
            var settings = CreateSettingsForBuilder(enableNullableReferenceTypes: false);
            var sut = new BuilderContext(CreateClass(), settings, CultureInfo.InvariantCulture);

            // Act & Assert
            Action a = () => sut.MapAttribute(attribute: null!);
            a.ShouldThrow<ArgumentNullException>()
             .ParamName.ShouldBe("attribute");
        }
    }
}

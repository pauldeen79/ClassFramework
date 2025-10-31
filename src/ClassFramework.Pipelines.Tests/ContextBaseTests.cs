namespace ClassFramework.Pipelines.Tests;

public class ContextBaseTests : TestBase
{
    protected static ContextBase<string> CreateSut(PipelineSettings settings) => new TestContext(settings);

    public class GetMappingMetadata : ContextBaseTests
    {
        [Fact]
        public void Throws_On_Null_TypeName()
        {
            // Arrange
            Action a = () => CreateSut(new PipelineSettingsBuilder()).GetMappingMetadata(typeName: null!);

            // Act & Assert
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("typeName");
        }

        [Fact]
        public void Returns_Empty_Sequence_When_No_Mapping_Matches_Input_TypeName()
        {
            // Arrange
            var typeName = "MyNamespace.MyClass";

            // Act
            var result = CreateSut(new PipelineSettingsBuilder()).GetMappingMetadata(typeName);

            // Assert
            result.ShouldBeEmpty();
        }

        [Fact]
        public void Returns_Mapping_From_Namespace_When_Mapping_Matches_Namespace_From_Input_TypeName()
        {
            // Arrange
            var additionalMetadata = new[] { new MetadataBuilder().WithName("MyName2") };
            var typeName = "MyNamespace.MyClass";
            var settings = new PipelineSettingsBuilder();
            settings.AddNamespaceMappings(new NamespaceMappingBuilder().WithSourceNamespace("MyNamespace").WithTargetNamespace("IgnoredNamespace").AddMetadata(additionalMetadata)); // this one gets ignored, as typename gets precedence
            settings.AddTypenameMappings(new TypenameMappingBuilder().WithSourceTypeName("MyNamespace.MyClass").WithTargetTypeName("MappedNamespace.MappedClass").AddMetadata(additionalMetadata));

            // Act
            var result = CreateSut(settings.Build()).GetMappingMetadata(typeName);

            // Assert
            result.ToArray().ShouldBeEquivalentTo(additionalMetadata.Select(x => x.Build()).ToArray());
        }

        [Fact]
        public void Returns_Mapping_From_Namespace_When_Mapping_Matches_TypeName_From_Input_TypeName()
        {
            // Arrange
            var additionalMetadata = new[] { new MetadataBuilder().WithName("MyName2") };
            var typeName = "MyNamespace.MyClass";
            var settings = new PipelineSettingsBuilder();
            settings.AddTypenameMappings(new TypenameMappingBuilder().WithSourceTypeName("MyNamespace.MyClass").WithTargetTypeName("MappedNamespace.MappedClass").AddMetadata(additionalMetadata));

            // Act
            var result = CreateSut(settings.Build()).GetMappingMetadata(typeName);

            // Assert
            result.ToArray().ShouldBeEquivalentTo(additionalMetadata.Select(x => x.Build()).ToArray());
        }
    }

    public class NullCheck : ContextBaseTests
    {
        [Theory]
        [InlineData(true, "is null")]
        [InlineData(false, "== null")]
        public void Returns_Correct_Result_Based_On_PatternMatching_Setting(bool usePatternMatchingForNullChecks, string expectedResult)
        {
            // Arrange
            var sut = CreateSut(new PipelineSettingsBuilder().WithUsePatternMatchingForNullChecks(usePatternMatchingForNullChecks).Build());

            // Act
            var result = sut.NullCheck;

            // Assert
            result.ShouldBe(expectedResult);
        }
    }

    public class NotNullCheck : ContextBaseTests
    {
        [Theory]
        [InlineData(true, "is not null")]
        [InlineData(false, "!= null")]
        public void Returns_Correct_Result_Based_On_PatternMatching_Setting(bool usePatternMatchingForNullChecks, string expectedResult)
        {
            // Arrange
            var sut = CreateSut(new PipelineSettingsBuilder().WithUsePatternMatchingForNullChecks(usePatternMatchingForNullChecks).Build());

            // Act
            var result = sut.NotNullCheck;

            // Assert
            result.ShouldBe(expectedResult);
        }
    }

    private sealed class TestContext(PipelineSettings settings) : ContextBase<string>(string.Empty, settings, CultureInfo.InvariantCulture, CancellationToken.None)
    {
        protected override string NewCollectionTypeName => string.Empty;

        public override object GetResponseBuilder()
        {
            throw new NotImplementedException();
        }

        public override bool SourceModelHasNoProperties()
        {
            throw new NotImplementedException();
        }
    }
}

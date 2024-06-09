namespace ClassFramework.Pipelines.Tests;

public class ContextBaseTests : TestBase
{
    protected ContextBase<string> CreateSut(PipelineSettings settings) => new TestContext(settings);

    public class GetMappingMetadata : ContextBaseTests
    {
        [Fact]
        public void Throws_On_Null_TypeName()
        {
            // Act & Assert
            CreateSut(new PipelineSettingsBuilder().Build())
                .Invoking(x => x.GetMappingMetadata(typeName: null!))
                .Should().Throw<ArgumentNullException>().WithParameterName("typeName");
        }

        [Fact]
        public void Returns_Empty_Sequence_When_No_Mapping_Matches_Input_TypeName()
        {
            // Arrange
            var typeName = "MyNamespace.MyClass";

            // Act
            var result = CreateSut(new PipelineSettingsBuilder().Build()).GetMappingMetadata(typeName);

            // Assert
            result.Should().BeEmpty();
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
            result.Should().BeEquivalentTo(additionalMetadata.Select(x => x.Build()));
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
            result.Should().BeEquivalentTo(additionalMetadata.Select(x => x.Build()));
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
            result.Should().Be(expectedResult);
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
            result.Should().Be(expectedResult);
        }
    }

    private sealed class TestContext : ContextBase<string>
    {
        public TestContext(PipelineSettings settings) : base(string.Empty, settings, CultureInfo.InvariantCulture)
        {
        }

        protected override string NewCollectionTypeName => string.Empty;
    }
}

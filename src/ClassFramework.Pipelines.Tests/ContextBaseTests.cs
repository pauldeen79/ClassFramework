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

    private sealed class TestContext : ContextBase<string>
    {
        public TestContext(PipelineSettings settings) : base(string.Empty, settings, CultureInfo.InvariantCulture)
        {
        }

        public override object CreateModel() => string.Empty;

        protected override string NewCollectionTypeName => string.Empty;
    }
}

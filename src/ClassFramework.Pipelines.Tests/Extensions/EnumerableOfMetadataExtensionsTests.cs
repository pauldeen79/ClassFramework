namespace ClassFramework.Pipelines.Tests.Extensions;

public class EnumerableOfMetadataExtensionsTests : TestBase
{
    public class WithMappingMetadata : EnumerableOfMetadataExtensionsTests
    {
        [Fact]
        public void Throws_On_Null_TypeName()
        {
            // Arrange
            var metadata = new[] { new MetadataBuilder().WithName("MyName").Build() };
            var pipelineBuilderTypeSettings = new PipelineSettingsBuilder();

            // Act & Assert
            metadata.Invoking(x => x.WithMappingMetadata(typeName: null!, pipelineBuilderTypeSettings.Build()))
                    .Should().Throw<ArgumentNullException>().WithParameterName("typeName");
        }

        [Fact]
        public void Throws_On_Null_Settings()
        {
            // Arrange
            var metadata = new[] { new MetadataBuilder().WithName("MyName").Build() };
            var typeName = "MyNamespace.MyClass";

            // Act & Assert
            metadata.Invoking(x => x.WithMappingMetadata(typeName, settings: null!))
                    .Should().Throw<ArgumentNullException>().WithParameterName("settings");
        }

        [Fact]
        public void Returns_InputData_When_No_Mapping_Matches_Input_TypeName()
        {
            // Arrange
            var metadata = new[] { new MetadataBuilder().WithName("MyName").Build() };
            var typeName = "MyNamespace.MyClass";
            var pipelineBuilderTypeSettings = new PipelineSettingsBuilder();

            // Act
            var result = metadata.WithMappingMetadata(typeName, pipelineBuilderTypeSettings.Build());

            // Assert
            result.Should().BeEquivalentTo(metadata);
        }

        [Fact]
        public void Returns_InputData_With_Mapping_From_Namespace_When_Mapping_Matches_Namespace_From_Input_TypeName()
        {
            // Arrange
            var metadata = new[] { new MetadataBuilder().WithName("MyName").Build() };
            var additionalMetadata = new[] { new MetadataBuilder().WithName("MyName2") };
            var typeName = "MyNamespace.MyClass";
            var pipelineBuilderTypeSettings = new PipelineSettingsBuilder();
            pipelineBuilderTypeSettings.AddNamespaceMappings(new NamespaceMappingBuilder().WithSourceNamespace("MyNamespace").WithTargetNamespace("IgnoredNamespace").AddMetadata(additionalMetadata)); // this one gets ignored, as typename gets precedence
            pipelineBuilderTypeSettings.AddTypenameMappings(new TypenameMappingBuilder().WithSourceTypeName("MyNamespace.MyClass").WithTargetTypeName("MappedNamespace.MappedClass").AddMetadata(additionalMetadata));

            // Act
            var result = metadata.WithMappingMetadata(typeName, pipelineBuilderTypeSettings.Build());

            // Assert
            result.Should().BeEquivalentTo(metadata.Concat(additionalMetadata.Select(x => x.Build())));
        }

        [Fact]
        public void Returns_InputData_With_Mapping_From_Namespace_When_Mapping_Matches_TypeName_From_Input_TypeName()
        {
            // Arrange
            var metadata = new[] { new MetadataBuilder().WithName("MyName").Build() };
            var additionalMetadata = new[] { new MetadataBuilder().WithName("MyName2") };
            var typeName = "MyNamespace.MyClass";
            var pipelineBuilderTypeSettings = new PipelineSettingsBuilder();
            pipelineBuilderTypeSettings.AddTypenameMappings(new TypenameMappingBuilder().WithSourceTypeName("MyNamespace.MyClass").WithTargetTypeName("MappedNamespace.MappedClass").AddMetadata(additionalMetadata));

            // Act
            var result = metadata.WithMappingMetadata(typeName, pipelineBuilderTypeSettings.Build());

            // Assert
            result.Should().BeEquivalentTo(metadata.Concat(additionalMetadata.Select(x => x.Build())));
        }
    }
}

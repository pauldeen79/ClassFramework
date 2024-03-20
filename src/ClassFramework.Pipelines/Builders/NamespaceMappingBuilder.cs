namespace ClassFramework.Pipelines.Builders;

public partial class NamespaceMappingBuilder
{
    public NamespaceMappingBuilder AddMetadata(string name, object? value) => AddMetadata(new MetadataBuilder().WithName(name.IsNotNull(nameof(name))).WithValue(value));
}

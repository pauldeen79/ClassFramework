namespace ClassFramework.Pipelines.Builders;

public partial class NamespaceMappingBuilder
{
    public NamespaceMappingBuilder(string sourceNamespace, string targetNamespace)
    {
        ArgumentGuard.IsNotNullOrEmpty(sourceNamespace, nameof(sourceNamespace));
        ArgumentGuard.IsNotNullOrEmpty(targetNamespace, nameof(targetNamespace));

        _sourceNamespace = sourceNamespace;
        _targetNamespace = targetNamespace;
        _metadata = [];
    }

    public NamespaceMappingBuilder(string @namespace)
    {
        ArgumentGuard.IsNotNullOrEmpty(@namespace, nameof(@namespace));

        _sourceNamespace = @namespace;
        _targetNamespace = @namespace;
        _metadata = [];
    }

    public NamespaceMappingBuilder AddMetadata(string name, object? value)
        => AddMetadata(new MetadataBuilder().WithName(name.IsNotNull(nameof(name))).WithValue(value));
}

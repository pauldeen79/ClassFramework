namespace ClassFramework.Pipelines.Extensions;

public static class EnumerableOfMetadataExtensions
{
    public static IEnumerable<Metadata> WithMappingMetadata(
        this IEnumerable<Metadata> metadata,
        string typeName,
        PipelineSettings settings)
    {
        typeName = typeName.IsNotNull(nameof(typeName)).FixTypeName();
        settings = settings.IsNotNull(nameof(settings));

        var typeNameMapping = settings.TypenameMappings.FirstOrDefault(x => x.SourceTypeName == typeName);
        if (typeNameMapping is not null)
        {
            return metadata.Concat(typeNameMapping.Metadata);
        }

        var ns = typeName.GetNamespaceWithDefault();
        if (!string.IsNullOrEmpty(ns))
        {
            var namespaceMapping = settings.NamespaceMappings.FirstOrDefault(x => x.SourceNamespace == ns);
            if (namespaceMapping is not null)
            {
                return metadata.Concat(namespaceMapping.Metadata);
            }
        }

        return metadata;
    }
}

namespace ClassFramework.Pipelines.Extensions;

public static class StringExtensions
{
    public static string MapTypeName(this string typeName, PipelineSettings settings, string newCollectionTypeName = "", string alternateTypeMetadataName = "")
    {
        settings = settings.IsNotNull(nameof(settings));
        newCollectionTypeName = newCollectionTypeName.IsNotNull(nameof(newCollectionTypeName));
        alternateTypeMetadataName = alternateTypeMetadataName.IsNotNull(nameof(alternateTypeMetadataName));
        var suffix = string.Empty;

        if (typeName.EndsWith("?"))
        {
            typeName = typeName.Substring(0, typeName.Length - 1);
            suffix = "?";
        }

        if (typeName.IsCollectionTypeName())
        {
            // i.e. IEnumerable<TSource> => IEnumerable<TTarget> (including collection typename mapping, when available)
            var newTypeName = typeName
                .FixCollectionTypeName(newCollectionTypeName) // note that this always converts to a generic type :)
                .ReplaceGenericTypeName(typeName.GetCollectionItemType().MapTypeName(settings, string.Empty, alternateTypeMetadataName)); // so we can safely use ReplaceGenericTypeName here

            return $"{newTypeName}{suffix}";
        }

        if (settings.InheritFromInterfaces && !string.IsNullOrEmpty(alternateTypeMetadataName))
        {
            return $"{MapTypeUsingAlternateTypeMetadata(typeName, settings, newCollectionTypeName, alternateTypeMetadataName)}{suffix}";
        }

        var genericArguments = typeName.FixTypeName().GetProcessedGenericArguments();
        if (!string.IsNullOrEmpty(genericArguments))
        {
            return $"{MapTypeUsingGenerics(typeName, settings, newCollectionTypeName, alternateTypeMetadataName, genericArguments)}{suffix}";
        }

        var typeNameMapping = settings.TypenameMappings.FirstOrDefault(x => x.SourceTypeName == typeName);
        if (typeNameMapping is not null)
        {
            // i.e. TSource => TTarget
            return $"{typeNameMapping.TargetTypeName}{suffix}";
        }

        var ns = typeName.GetNamespaceWithDefault();
        if (!string.IsNullOrEmpty(ns))
        {
            // i.e. SourceNamespace.T => TargetNamespace.T
            var namespaceMapping = settings.NamespaceMappings.FirstOrDefault(x => x.SourceNamespace == ns);
            if (namespaceMapping is not null)
            {
                return $"{namespaceMapping.TargetNamespace}.{typeName.GetClassName()}{suffix}";
            }
        }

        return $"{typeName}{suffix}";
    }

    private static string MapTypeUsingGenerics(string typeName, PipelineSettings settings, string newCollectionTypeName, string alternateTypeMetadataName, string genericArguments)
    {
        var mappedGenericArgumentsBuilder = new StringBuilder();
        foreach (var item in genericArguments.Split(',').Select(x => x.Trim()))
        {
            if (mappedGenericArgumentsBuilder.Length > 0)
            {
                mappedGenericArgumentsBuilder.Append(",");
            }

            mappedGenericArgumentsBuilder.Append(item.MapTypeName(settings, string.Empty, alternateTypeMetadataName));
        }

        return $"{typeName.FixTypeName().WithoutProcessedGenerics().MapTypeName(settings, newCollectionTypeName, alternateTypeMetadataName)}<{mappedGenericArgumentsBuilder}>";
    }

    private static string MapTypeUsingAlternateTypeMetadata(string typeName, PipelineSettings settings, string newCollectionTypeName, string alternateTypeMetadataName)
    {
        var typenameMapping = settings.TypenameMappings.FirstOrDefault(x => x.SourceTypeName == (typeName.IsCollectionTypeName()
            ? typeName.GetProcessedGenericArguments()
            : typeName));
        if (typenameMapping is not null)
        {
            var alternateType = typenameMapping.Metadata.GetStringValue(alternateTypeMetadataName);
            if (!string.IsNullOrEmpty(alternateType))
            {
                return alternateType;
            }
        }

        return typeName.MapTypeName(settings, newCollectionTypeName, string.Empty);
    }

    public static string MapNamespace(this string? ns, PipelineSettings settings)
    {
        settings = settings.IsNotNull(nameof(settings));

        if (!string.IsNullOrEmpty(ns))
        {
            // i.e. SourceNamespace.T => TargetNamespace.T
            var namespaceMapping = settings.NamespaceMappings.FirstOrDefault(x => x.SourceNamespace == ns);
            if (namespaceMapping is not null)
            {
                return namespaceMapping.TargetNamespace;
            }
        }

        return ns ?? string.Empty;
    }

    public static string FixNullableTypeName(this string typeName, ITypeContainer typeContainer)
    {
        typeContainer = typeContainer.IsNotNull(nameof(typeContainer));

        if (!typeName.StartsWith("System.Nullable", StringComparison.Ordinal) && typeContainer.IsNullable && typeContainer.IsValueType)
        {
            return typeof(Nullable<>).ReplaceGenericTypeName(typeName);
        }

        return typeName;
    }

    public static string AppendWhenNotNullOrEmpty(this string? instance, string valueToAppend)
    {
        if (string.IsNullOrEmpty(instance))
        {
            return string.Empty;
        }

        return string.Concat(instance!, valueToAppend);
    }
}

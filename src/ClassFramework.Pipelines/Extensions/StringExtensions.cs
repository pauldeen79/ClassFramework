﻿namespace ClassFramework.Pipelines.Extensions;

public static class StringExtensions
{
    public static string MapTypeName(this string typeName, PipelineSettings settings, string newCollectionTypeName = "", string alternateTypeMetadataName = "")
    {
        settings = settings.IsNotNull(nameof(settings));
        newCollectionTypeName = newCollectionTypeName.IsNotNull(nameof(newCollectionTypeName));
        alternateTypeMetadataName = alternateTypeMetadataName.IsNotNull(nameof(alternateTypeMetadataName));

        if (typeName.IsCollectionTypeName())
        {
            // i.e. IEnumerable<TSource> => IEnumerable<TTarget> (including collection typename mapping, when available)
            return typeName
                .FixCollectionTypeName(newCollectionTypeName) // note that this always converts to a generic type :)
                .ReplaceGenericTypeName(MapTypeName(typeName.GetCollectionItemType(), settings, newCollectionTypeName, alternateTypeMetadataName)); // so we can safely use ReplaceGenericTypeName here
        }

        if (settings.InheritFromInterfaces && !string.IsNullOrEmpty(alternateTypeMetadataName))
        {
            var typenameMapping = settings.TypenameMappings.FirstOrDefault(x => x.SourceTypeName == (typeName.IsCollectionTypeName() ? typeName.GetGenericArguments() : typeName));
            if (typenameMapping is not null)
            {
                var alternateType = typenameMapping.Metadata.GetStringValue(alternateTypeMetadataName);
                if (!string.IsNullOrEmpty(alternateType))
                {
                    return alternateType;
                }
            }
        }

        var genericArguments = typeName.FixTypeName().GetProcessedGenericArguments();
        if (!string.IsNullOrEmpty(genericArguments))
        {
            var mappedGenericArgumentsBuilder = new StringBuilder();
            foreach (var item in genericArguments.Split(',').Select(x => x.Trim()))
            {
                if (mappedGenericArgumentsBuilder.Length > 0)
                {
                    mappedGenericArgumentsBuilder.Append(",");
                }

                mappedGenericArgumentsBuilder.Append(MapTypeName(item, settings, newCollectionTypeName, alternateTypeMetadataName));
            }

            return $"{MapTypeName(typeName.FixTypeName().WithoutProcessedGenerics(), settings, newCollectionTypeName, alternateTypeMetadataName)}<{mappedGenericArgumentsBuilder}>";
        }

        var typeNameMapping = settings.TypenameMappings.FirstOrDefault(x => x.SourceTypeName == typeName);
        if (typeNameMapping is not null)
        {
            // i.e. TSource => TTarget
            return typeNameMapping.TargetTypeName;
        }

        var ns = typeName.GetNamespaceWithDefault();
        if (!string.IsNullOrEmpty(ns))
        {
            // i.e. SourceNamespace.T => TargetNamespace.T
            var namespaceMapping = settings.NamespaceMappings.FirstOrDefault(x => x.SourceNamespace == ns);
            if (namespaceMapping is not null)
            {
                return $"{namespaceMapping.TargetNamespace}.{typeName.GetClassName()}";
            }
        }

        return typeName;
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

namespace ClassFramework.Pipelines.Extensions;

public static class PropertyExtensions
{
    public static string GetDefaultValue(this Property property, ICsharpExpressionCreator csharpExpressionCreator, bool enableNullableReferenceTypes, string typeName, PipelineSettings settings)
    {
        csharpExpressionCreator = csharpExpressionCreator.IsNotNull(nameof(csharpExpressionCreator));

        var md = property.Metadata
            .WithMappingMetadata(property.TypeName.GetCollectionItemType().WhenNullOrEmpty(property.TypeName), settings)
            .FirstOrDefault(x => x.Name == MetadataNames.CustomBuilderDefaultValue);

        if (md is not null && md.Value is not null)
        {
            if (md.Value is Literal literal && literal.Value is not null)
            {
                return literal.Value;
            }

            return csharpExpressionCreator.Create(md.Value);
        }

        return typeName.GetDefaultValue(property.IsNullable, property.IsValueType, enableNullableReferenceTypes);
    }

    public static string GetNullCheckSuffix(this Property property, string name, bool addNullChecks)
    {
        if (!addNullChecks || property.IsNullable || property.IsValueType)
        {
            return string.Empty;
        }

        return $" ?? throw new {typeof(ArgumentNullException).FullName}(nameof({name}))";
    }

    public static string GetBuilderMemberName(this Property property, bool addNullChecks, bool enableNullableReferenceTypes, ArgumentValidationType argumentValidation, bool addBackingFields, CultureInfo cultureInfo)
    {
        cultureInfo = cultureInfo.IsNotNull(nameof(cultureInfo));

        if (property.HasBackingFieldOnBuilder(addNullChecks, enableNullableReferenceTypes, argumentValidation, addBackingFields))
        {
            return $"_{property.Name.ToPascalCase(cultureInfo)}";
        }

        return property.Name;
    }

    public static string GetEntityMemberName(this Property property, bool addBackingFields, CultureInfo cultureInfo)
    {
        cultureInfo = cultureInfo.IsNotNull(nameof(cultureInfo));

        if (addBackingFields)
        {
            return $"_{property.Name.ToPascalCase(cultureInfo)}";
        }

        return property.Name;
    }

    // For now, only add backing fields for non nullable fields.
    // Nullable fields can simply have auto properties, as null checks are not needed
    public static bool HasBackingFieldOnBuilder(this Property property, bool addNullChecks, bool enableNullableReferenceTypes, ArgumentValidationType argumentValidation, bool addBackingFields)
        => (addNullChecks
        && !property.IsValueType
        && !property.IsNullable(enableNullableReferenceTypes)
        && argumentValidation != ArgumentValidationType.Shared) || addBackingFields;

    public static Result<string> GetBuilderConstructorInitializer(
        this Property property,
        PipelineSettings settings,
        IFormatProvider formatProvider,
        object parentChildContext,
        string mappedTypeName,
        string newCollectionTypeName,
        string metadataName,
        IFormattableStringParser formattableStringParser)
    {
        settings = settings.IsNotNull(nameof(settings));
        formatProvider = formatProvider.IsNotNull(nameof(formatProvider));
        parentChildContext = parentChildContext.IsNotNull(nameof(parentChildContext));
        mappedTypeName = mappedTypeName.IsNotNull(nameof(mappedTypeName));
        newCollectionTypeName = newCollectionTypeName.IsNotNull(nameof(newCollectionTypeName));
        metadataName = metadataName.IsNotNull(nameof(metadataName));
        formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

        var builderArgumentTypeResult = GetBuilderArgumentTypeName(property, settings, formatProvider, parentChildContext, mappedTypeName, formattableStringParser);

        if (!builderArgumentTypeResult.IsSuccessful())
        {
            return builderArgumentTypeResult;
        }

        var customBuilderConstructorInitializeExpression = string.IsNullOrEmpty(metadataName)
            ? string.Empty
            : property.Metadata
                .WithMappingMetadata(property.TypeName.GetCollectionItemType().WhenNullOrEmpty(property.TypeName), settings)
                .GetStringValue(metadataName);

        var result = formattableStringParser.Parse(customBuilderConstructorInitializeExpression, formatProvider, parentChildContext);
        if (!result.IsSuccessful())
        {
            return result;
        }

        return Result.Success(builderArgumentTypeResult.Value!
            .FixCollectionTypeName(newCollectionTypeName)
            .GetCollectionInitializeStatement(result.Value?.Replace("source.[Name]", "x").Replace("[Name]", property.Name) ?? string.Empty).Replace("[Name]", property.Name)
            .GetCsharpFriendlyTypeName());
    }

    public static Result<string> GetBuilderArgumentTypeName(
        this Property property,
        PipelineSettings settings,
        IFormatProvider formatProvider,
        object parentChildContext,
        string mappedTypeName,
        IFormattableStringParser formattableStringParser)
    {
        settings = settings.IsNotNull(nameof(settings));
        formatProvider = formatProvider.IsNotNull(nameof(formatProvider));
        parentChildContext = parentChildContext.IsNotNull(nameof(parentChildContext));
        mappedTypeName = mappedTypeName.IsNotNull(nameof(mappedTypeName));
        formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

        var metadata = property.Metadata.WithMappingMetadata(property.TypeName.GetCollectionItemType().WhenNullOrEmpty(property.TypeName), settings);
        var ns = metadata.GetStringValue(MetadataNames.CustomBuilderNamespace);

        if (!string.IsNullOrEmpty(ns))
        {
            var newTypeName = metadata.GetStringValue(MetadataNames.CustomBuilderName, "{TypeName}");
            var newFullName = $"{ns}.{newTypeName}";
            if (property.TypeName.IsCollectionTypeName())
            {
                var idx = property.TypeName.IndexOf('<');
                if (idx > -1)
                {
                    newFullName = $"{property.TypeName.Substring(0, idx)}<{newFullName.Replace("{TypeName.ClassName}", "{TypeName.GenericArguments.ClassName}")}>";
                }
            }

            return formattableStringParser.Parse
            (
                newFullName,
                formatProvider,
                parentChildContext
            );
        }

        return formattableStringParser.Parse
        (
            metadata.GetStringValue(MetadataNames.CustomBuilderArgumentType, mappedTypeName),
            formatProvider,
            parentChildContext
        );
    }

    public static Result<string> GetBuilderParentTypeName(this Property property, PipelineContext<IConcreteTypeBuilder, BuilderContext> context, IFormattableStringParser formattableStringParser)
    {
        context = context.IsNotNull(nameof(context));
        formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

        if (string.IsNullOrEmpty(property.ParentTypeFullName))
        {
            return Result.Success(property.ParentTypeFullName);
        }

        var metadata = property.Metadata.WithMappingMetadata(property.ParentTypeFullName.GetCollectionItemType().WhenNullOrEmpty(property.ParentTypeFullName), context.Context.Settings);
        var ns = metadata.GetStringValue(MetadataNames.CustomBuilderParentTypeNamespace);

        if (string.IsNullOrEmpty(ns))
        {
            return Result.Success(context.Context.MapTypeName(property.ParentTypeFullName.FixTypeName()));
        }

        var newTypeName = metadata.GetStringValue(MetadataNames.CustomBuilderParentTypeName, "{ParentTypeName.ClassName}");

        if (property.TypeName.IsCollectionTypeName())
        {
            newTypeName = newTypeName.Replace("{TypeName.ClassName}", "{TypeName.GenericArguments.ClassName}");
        }

        var newFullName = $"{ns}.{newTypeName}";

        return formattableStringParser.Parse
        (
            newFullName,
            context.Context.FormatProvider,
            new ParentChildContext<PipelineContext<IConcreteTypeBuilder, BuilderContext>, Property>(context, property, context.Context.Settings)
        );
    }

    public static string GetSuffix(this Property source, bool enableNullableReferenceTypes)
        => source.IsNullable(enableNullableReferenceTypes) && !source.IsValueType && !source.TypeName.IsCollectionTypeName()
            ? "?"
            : string.Empty;
}

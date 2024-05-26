namespace ClassFramework.Pipelines.Extensions;

public static class PropertyExtensions
{
    public static string GetDefaultValue<TSourceModel>(this Property property, ICsharpExpressionDumper csharpExpressionDumper, string typeName, ContextBase<TSourceModel> context)
    {
        csharpExpressionDumper = csharpExpressionDumper.IsNotNull(nameof(csharpExpressionDumper));
        context = context.IsNotNull(nameof(context));

        var suffix = !property.IsValueType && !property.IsNullable && context.Settings.EnableNullableReferenceTypes
            ? "!"
            : string.Empty;

        var defaultValueAttribute = property.Attributes.FirstOrDefault(x => x.Name == typeof(DefaultValueAttribute).FullName);
        if (defaultValueAttribute is not null)
        {
            var value = defaultValueAttribute.Parameters.Single().Value;
            if (value is Literal literal && literal.Value is not null)
            {
                value = new StringLiteral(literal.Value);
            }

            return $"{csharpExpressionDumper.Dump(value)}{suffix}";
        }

        var md = context
            .GetMappingMetadata(property.TypeName)
            .LastOrDefault(x => x.Name == MetadataNames.CustomBuilderDefaultValue);

        if (md is not null && md.Value is not null)
        {
            var value = md.Value;
            if (value is Literal literal && literal.Value is not null)
            {

                value = new StringLiteral(literal.Value);
            }

            return $"{csharpExpressionDumper.Dump(value)}{suffix}";
        }

        return typeName.GetDefaultValue(property.IsNullable, property.IsValueType, context.Settings.EnableNullableReferenceTypes);
    }

    public static string GetNullCheckSuffix(this Property property, string name, bool addNullChecks, IType sourceModel)
    {
        name = name.IsNotNull(nameof(name));
        sourceModel = sourceModel.IsNotNull(nameof(sourceModel));

        // note that for now, we assume that a generic type argument should not be included in argument null checks...
        // this might be the case (for example there is a constraint on class), but this is not supported yet
        var isGenericArgument = sourceModel.GenericTypeArguments.Contains(property.TypeName);

        if (!addNullChecks || property.IsNullable || property.IsValueType || isGenericArgument)
        {
            return string.Empty;
        }

        return $" ?? throw new {typeof(ArgumentNullException).FullName}(nameof({name}))";
    }

    public static string GetBuilderMemberName(this Property property, PipelineSettings settings, CultureInfo cultureInfo)
    {
        cultureInfo = cultureInfo.IsNotNull(nameof(cultureInfo));

        if (property.HasBackingFieldOnBuilder(settings))
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
    public static bool HasBackingFieldOnBuilder(this Property property, PipelineSettings settings)
    {
        settings = settings.IsNotNull(nameof(settings));
        return (settings.AddNullChecks && !property.IsValueType && !property.IsNullable(settings.EnableNullableReferenceTypes))
            || settings.AddBackingFields
            || settings.CreateAsObservable;
    }

    public static Result<FormattableStringParserResult> GetBuilderConstructorInitializer<TSourceModel>(
        this Property property,
        ContextBase<TSourceModel> context,
        object parentChildContext,
        string mappedTypeName,
        string newCollectionTypeName,
        string metadataName,
        IFormattableStringParser formattableStringParser)
    {
        context = context.IsNotNull(nameof(context));
        parentChildContext = parentChildContext.IsNotNull(nameof(parentChildContext));
        mappedTypeName = mappedTypeName.IsNotNull(nameof(mappedTypeName));
        newCollectionTypeName = newCollectionTypeName.IsNotNull(nameof(newCollectionTypeName));
        metadataName = metadataName.IsNotNull(nameof(metadataName));
        formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

        var builderArgumentTypeResult = GetBuilderArgumentTypeName(property, context, parentChildContext, mappedTypeName, formattableStringParser);

        if (!builderArgumentTypeResult.IsSuccessful())
        {
            return builderArgumentTypeResult;
        }

        var customBuilderConstructorInitializeExpression = string.IsNullOrEmpty(metadataName)
            ? string.Empty
            : context
                .GetMappingMetadata(property.TypeName)
                .GetStringValue(metadataName);

        var result = formattableStringParser.Parse(customBuilderConstructorInitializeExpression, context.FormatProvider, parentChildContext);
        if (!result.IsSuccessful())
        {
            return result;
        }

        return Result.Success<FormattableStringParserResult>(builderArgumentTypeResult.Value!.ToString()
            .FixCollectionTypeName(newCollectionTypeName)
            .GetCollectionInitializeStatement(result.Value?.ToString().Replace("source.[Name]", "x").Replace("[Name]", property.Name) ?? string.Empty).Replace("[Name]", property.Name)
            .GetCsharpFriendlyTypeName());
    }

    public static Result<FormattableStringParserResult> GetBuilderArgumentTypeName<TSourceModel>(
        this Property property,
        ContextBase<TSourceModel> context,
        object parentChildContext,
        string mappedTypeName,
        IFormattableStringParser formattableStringParser)
    {
        context = context.IsNotNull(nameof(context));
        parentChildContext = parentChildContext.IsNotNull(nameof(parentChildContext));
        mappedTypeName = mappedTypeName.IsNotNull(nameof(mappedTypeName));
        formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

        var metadata = context.GetMappingMetadata(property.TypeName);
        var ns = metadata.GetStringValue(MetadataNames.CustomBuilderNamespace);

        if (!string.IsNullOrEmpty(ns))
        {
            var newTypeName = metadata.GetStringValue(MetadataNames.CustomBuilderName, "{TypeName}");
            var newFullName = $"{ns}.{newTypeName}";
            if (property.TypeName.FixTypeName().IsCollectionTypeName())
            {
                var idx = property.TypeName.IndexOf('<');
                if (idx > -1)
                {
                    if (!string.IsNullOrEmpty(property.TypeName.FixTypeName().GetCollectionItemType().GetProcessedGenericArguments()))
                    {
                        newFullName = $"{property.TypeName.Substring(0, idx)}<{newFullName.Replace("{TypeName.ClassName}", "{TypeName.GenericArguments.ClassName}").Replace("{TypeName.ClassName.NoGenerics}", "{TypeName.GenericArguments.ClassName.NoGenerics}").Replace("{TypeName.GenericArgumentsWithBrackets}", "{TypeName.CollectionItemType.GenericArgumentsWithBrackets}")}>";
                    }
                    else
                    {
                        newFullName = $"{property.TypeName.Substring(0, idx)}<{newFullName.Replace("{TypeName.ClassName}", "{TypeName.GenericArguments.ClassName}").Replace("{TypeName.ClassName.NoGenerics}", "{TypeName.GenericArguments.ClassName.NoGenerics}")}>";
                    }
                }
            }

            return formattableStringParser.Parse
            (
                newFullName,
                context.FormatProvider,
                parentChildContext
            );
        }

        return formattableStringParser.Parse
        (
            metadata.GetStringValue(MetadataNames.CustomBuilderArgumentType, mappedTypeName),
            context.FormatProvider,
            parentChildContext
        );
    }

    public static Result<FormattableStringParserResult> GetBuilderParentTypeName(this Property property, PipelineContext<BuilderContext> context, IFormattableStringParser formattableStringParser)
    {
        context = context.IsNotNull(nameof(context));
        formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

        if (string.IsNullOrEmpty(property.ParentTypeFullName))
        {
            return Result.Success<FormattableStringParserResult>(property.ParentTypeFullName);
        }

        var metadata = context.Request.GetMappingMetadata(property.ParentTypeFullName);
        var ns = metadata.GetStringValue(MetadataNames.CustomBuilderParentTypeNamespace);

        if (string.IsNullOrEmpty(ns))
        {
            return Result.Success<FormattableStringParserResult>(context.Request.MapTypeName(property.ParentTypeFullName.FixTypeName()));
        }

        var newTypeName = metadata.GetStringValue(MetadataNames.CustomBuilderParentTypeName, "{ParentTypeName.ClassName}");

        if (property.TypeName.FixTypeName().IsCollectionTypeName())
        {
            newTypeName = newTypeName.Replace("{TypeName.ClassName}", "{TypeName.GenericArguments.ClassName}");
        }

        var newFullName = $"{ns}.{newTypeName}";

        return formattableStringParser.Parse
        (
            newFullName,
            context.Request.FormatProvider,
            new ParentChildContext<PipelineContext<BuilderContext>, Property>(context, property, context.Request.Settings)
        );
    }

    public static string GetSuffix<TSourceModel>(this Property source, bool enableNullableReferenceTypes, ICsharpExpressionDumper csharpExpressionDumper, ContextBase<TSourceModel> context)
        => CollectionIsValidForSuffix(source, enableNullableReferenceTypes)
        || NonCollectionIsValidForSuffix(source, csharpExpressionDumper, context)
            ? "?"
            : string.Empty;

    private static bool NonCollectionIsValidForSuffix<TSourceModel>(Property source, ICsharpExpressionDumper csharpExpressionDumper, ContextBase<TSourceModel> context)
        => !source.TypeName.IsCollectionTypeName()
        && !source.IsValueType
        && source.GetDefaultValue(csharpExpressionDumper, source.TypeName.FixTypeName(), context).StartsWith("default(");

    private static bool CollectionIsValidForSuffix(Property source, bool enableNullableReferenceTypes)
        => source.IsNullable(enableNullableReferenceTypes)
        && !source.IsValueType
        && !source.TypeName.FixTypeName().IsCollectionTypeName();
}

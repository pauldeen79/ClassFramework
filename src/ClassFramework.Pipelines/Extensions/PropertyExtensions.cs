namespace ClassFramework.Pipelines.Extensions;

public static class PropertyExtensions
{
    public static string GetDefaultValue(this Property property, ICsharpExpressionDumper csharpExpressionDumper, string typeName, MappedContextBase context)
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

        var metadata = context.GetMappingMetadata(property.TypeName).ToArray();

        var md = metadata.LastOrDefault(x => x.Name == MetadataNames.CustomBuilderDefaultValue);
        if (md is not null && md.Value is not null)
        {
            var value = md.Value;
            if (value is Literal literal && literal.Value is not null)
            {

                value = new StringLiteral(literal.Value);
            }

            return $"{csharpExpressionDumper.Dump(value)}{suffix}";
        }

        var useBuilderLazyValues = context.UseBuilderLazyValues(metadata);

        return typeName.GetDefaultValue(property.IsNullable, property.IsValueType, context.Settings.EnableNullableReferenceTypes, useBuilderLazyValues);
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
            return $"_{property.Name.ToCamelCase(cultureInfo)}";
        }

        return property.Name;
    }

    public static string GetEntityMemberName(this Property property, bool addBackingFields, CultureInfo cultureInfo)
    {
        cultureInfo = cultureInfo.IsNotNull(nameof(cultureInfo));

        if (addBackingFields)
        {
            return $"_{property.Name.ToCamelCase(cultureInfo)}";
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

    public static async Task<Result<GenericFormattableString>> GetBuilderConstructorInitializerAsync<TSourceModel>(
        this Property property,
        ContextBase<TSourceModel> context,
        object parentChildContext,
        string mappedTypeName,
        string newCollectionTypeName,
        string metadataName,
        IExpressionEvaluator evaluator)
    {
        context = context.IsNotNull(nameof(context));
        parentChildContext = parentChildContext.IsNotNull(nameof(parentChildContext));
        mappedTypeName = mappedTypeName.IsNotNull(nameof(mappedTypeName));
        newCollectionTypeName = newCollectionTypeName.IsNotNull(nameof(newCollectionTypeName));
        metadataName = metadataName.IsNotNull(nameof(metadataName));
        evaluator = evaluator.IsNotNull(nameof(evaluator));

        var builderArgumentTypeResult = await GetBuilderArgumentTypeNameAsync(property, context, parentChildContext, mappedTypeName, evaluator, context.CancellationToken).ConfigureAwait(false);
        if (!builderArgumentTypeResult.IsSuccessful())
        {
            return builderArgumentTypeResult;
        }

        var customBuilderConstructorInitializeExpression = string.IsNullOrEmpty(metadataName)
            ? string.Empty
            : context
                .GetMappingMetadata(property.TypeName)
                .GetStringValue(metadataName);

        var result = await evaluator.EvaluateInterpolatedStringAsync(customBuilderConstructorInitializeExpression, context.FormatProvider, parentChildContext, context.CancellationToken).ConfigureAwait(false);
        if (!result.IsSuccessful())
        {
            return result;
        }

        return Result.Success<GenericFormattableString>(builderArgumentTypeResult.Value!.ToString()
            .FixCollectionTypeName(newCollectionTypeName)
            .GetCollectionInitializeStatement(result.Value?.ToString().Replace($"source.{PlaceholderNames.NamePlaceholder}", "x").Replace(PlaceholderNames.NamePlaceholder, property.Name) ?? string.Empty).Replace(PlaceholderNames.NamePlaceholder, property.Name)
            .GetCsharpFriendlyTypeName());
    }

    private const string DefaultBuilderArgumentType = "{NoGenerics(property.TypeName)}";
    private const string ClassNameFormatString = "{ClassName(property.TypeName)}";
    private const string ClassNameGenericArgumentsFormatString = "{ClassName(GenericArguments(property.TypeName))}";
    private const string NoGenericsClassNameFormatString = "{NoGenerics(ClassName(property.TypeName))}";
    private const string NoGenericsClassNameGenericArgumentsFormatString = "{NoGenerics(ClassName(GenericArguments(property.TypeName)))}";

    public static Task<Result<GenericFormattableString>> GetBuilderArgumentTypeNameAsync<TSourceModel>(
        this Property property,
        ContextBase<TSourceModel> context,
        object parentChildContext,
        string mappedTypeName,
        IExpressionEvaluator evaluator,
        CancellationToken token)
        => GetBuilderArgumentTypeNameAsync(property, context, parentChildContext, mappedTypeName, evaluator, false, token);

    public static async Task<Result<GenericFormattableString>> GetBuilderArgumentTypeNameAsync<TSourceModel>(
        this Property property,
        ContextBase<TSourceModel> context,
        object parentChildContext,
        string mappedTypeName,
        IExpressionEvaluator evaluator,
        bool forceNonLazy,
        CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));
        parentChildContext = parentChildContext.IsNotNull(nameof(parentChildContext));
        mappedTypeName = mappedTypeName.IsNotNull(nameof(mappedTypeName));
        evaluator = evaluator.IsNotNull(nameof(evaluator));

        var metadata = context.GetMappingMetadata(property.TypeName).ToArray();
        var ns = metadata.GetStringValue(MetadataNames.CustomBuilderNamespace);

        if (!string.IsNullOrEmpty(ns))
        {
            var builderName = metadata.GetStringValue(MetadataNames.CustomBuilderName, ContextBase.DefaultBuilderName);
            var newFullName = $"{ns}.{builderName}";
            var useBuilderLazyValues = context.UseBuilderLazyValues(metadata);
            if (property.TypeName.FixTypeName().IsCollectionTypeName())
            {
                newFullName = GetFullNameForCollectionPropertyWithCustomBuilderNamespace(property, newFullName, useBuilderLazyValues);
            }
            else if (useBuilderLazyValues)
            {
                newFullName = newFullName.ToLazy();
            }

            return await evaluator.EvaluateInterpolatedStringAsync
            (
                newFullName,
                context.FormatProvider,
                parentChildContext,
                token
            ).ConfigureAwait(false);
        }

        if (!forceNonLazy && context.UseBuilderLazyValues(metadata))
        {
            var builderArgumentType = metadata.GetStringValue(MetadataNames.CustomBuilderArgumentType, DefaultBuilderArgumentType);
            if (property.TypeName.FixTypeName().IsCollectionTypeName())
            {
                builderArgumentType = GetFullNameForCollectionPropertyWithoutCustomBuilderNamespace(property, builderArgumentType);
            }
            else
            {
                builderArgumentType = builderArgumentType.ToLazy();
            }

            return await evaluator.EvaluateInterpolatedStringAsync
            (
                builderArgumentType,
                context.FormatProvider,
                parentChildContext,
                token
            ).ConfigureAwait(false);
        }

        // No custom builder namespace, no lazy values
        return await evaluator.EvaluateInterpolatedStringAsync
        (
            metadata.GetStringValue(MetadataNames.CustomBuilderArgumentType, mappedTypeName),
            context.FormatProvider,
            parentChildContext,
            token
        ).ConfigureAwait(false);
    }

    private static string GetFullNameForCollectionPropertyWithoutCustomBuilderNamespace(Property property, string builderArgumentType)
    {
        var idx = property.TypeName.IndexOf('<');
        if (idx == -1)
        {
            return builderArgumentType;
        }

        if (!string.IsNullOrEmpty(property.TypeName.FixTypeName().GetCollectionItemType().GetGenericArguments()))
        {
            var name = builderArgumentType
                .Replace("{property.TypeName}", "{GenericArguments(property.TypeName)}".ToLazy())
                .Replace("{NoGenerics(property.TypeName)}", "{NoGenerics(GenericArguments(property.TypeName))}".ToLazy())
                .Replace("{GenericArguments(property.TypeName, true)}", "{GenericArguments(CollectionItemType(property.TypeName), true)}");
            builderArgumentType = $"{property.TypeName.Substring(0, idx)}<{name}>";
        }
        else
        {
            var name = builderArgumentType
                .Replace("{property.TypeName}", "{GenericArguments(property.TypeName)}".ToLazy())
                .Replace("{NoGenerics(property.TypeName)}", "{NoGenerics(GenericArguments(property.TypeName))}".ToLazy());
            builderArgumentType = $"{property.TypeName.Substring(0, idx)}<{name}>";
        }

        return builderArgumentType;
    }

    private static string GetFullNameForCollectionPropertyWithCustomBuilderNamespace(Property property, string newFullName, bool useBuilderLazyValues)
    {
        var idx = property.TypeName.IndexOf('<');
        if (idx == -1)
        {
            return newFullName;
        }

        if (!string.IsNullOrEmpty(property.TypeName.FixTypeName().GetCollectionItemType().GetGenericArguments()))
        {
            return useBuilderLazyValues
                ? $"{property.TypeName.Substring(0, idx)}<{newFullName.Replace(ClassNameFormatString, ClassNameGenericArgumentsFormatString).Replace(NoGenericsClassNameFormatString, NoGenericsClassNameGenericArgumentsFormatString).Replace("{GenericArguments(property.TypeName, true)}", "{GenericArguments(CollectionItemType(property.TypeName), true)}").ToLazy()}>"
                : $"{property.TypeName.Substring(0, idx)}<{newFullName.Replace(ClassNameFormatString, ClassNameGenericArgumentsFormatString).Replace(NoGenericsClassNameFormatString, NoGenericsClassNameGenericArgumentsFormatString).Replace("{GenericArguments(property.TypeName, true)}", "{GenericArguments(CollectionItemType(property.TypeName), true)}")}>";
        }
        else
        {
            return useBuilderLazyValues
                ? $"{property.TypeName.Substring(0, idx)}<{newFullName.Replace(ClassNameFormatString, ClassNameGenericArgumentsFormatString).Replace(NoGenericsClassNameFormatString, NoGenericsClassNameGenericArgumentsFormatString).ToLazy()}>"
                : $"{property.TypeName.Substring(0, idx)}<{newFullName.Replace(ClassNameFormatString, ClassNameGenericArgumentsFormatString).Replace(NoGenericsClassNameFormatString, NoGenericsClassNameGenericArgumentsFormatString)}>";
        }
    }

    public static async Task<Result<GenericFormattableString>> GetBuilderParentTypeNameAsync(this Property property, PipelineContext<BuilderContext> context, IExpressionEvaluator evaluator, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));
        evaluator = evaluator.IsNotNull(nameof(evaluator));

        if (string.IsNullOrEmpty(property.ParentTypeFullName))
        {
            return Result.Success<GenericFormattableString>(property.ParentTypeFullName);
        }

        var metadata = context.Request.GetMappingMetadata(property.ParentTypeFullName).ToArray();
        var ns = metadata.GetStringValue(MetadataNames.CustomBuilderParentTypeNamespace);
        if (string.IsNullOrEmpty(ns))
        {
            return Result.Success<GenericFormattableString>(context.Request.MapTypeName(property.ParentTypeFullName.FixTypeName()));
        }

        var newTypeName = metadata.GetStringValue(MetadataNames.CustomBuilderParentTypeName, "{ClassName(property.ParentTypeFullName)}");
        if (property.TypeName.FixTypeName().IsCollectionTypeName())
        {
            newTypeName = newTypeName.Replace(ClassNameFormatString, ClassNameGenericArgumentsFormatString);
        }

        var newFullName = $"{ns}.{newTypeName}";

        return await evaluator.EvaluateInterpolatedStringAsync
        (
            newFullName,
            context.Request.FormatProvider,
            new ParentChildContext<PipelineContext<BuilderContext>, Property>(context, property, context.Request.Settings),
            token
        ).ConfigureAwait(false);
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

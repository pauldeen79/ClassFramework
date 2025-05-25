namespace ClassFramework.Pipelines;

public abstract class ContextBase(PipelineSettings settings, IFormatProvider formatProvider)
{
    public PipelineSettings Settings { get; } = settings.IsNotNull(nameof(settings));
    public IFormatProvider FormatProvider { get; } = formatProvider.IsNotNull(nameof(formatProvider));

    public string NullCheck => Settings.UsePatternMatchingForNullChecks
        ? "is null"
        : "== null";

    public string NotNullCheck => Settings.UsePatternMatchingForNullChecks
        ? "is not null"
        : "!= null";

    public string CreateArgumentNullException(string argumentName)
    {
        if (Settings.UseExceptionThrowIfNull)
        {
            return $"{typeof(ArgumentNullException).FullName}.ThrowIfNull({argumentName});";
        }

        return $"if ({argumentName} {NullCheck}) throw new {typeof(ArgumentNullException).FullName}(nameof({argumentName}));";
    }

    public string MapNamespace(string? ns)
        => ns.MapNamespace(Settings);

    public void AddNullChecks(MethodBuilder builder, IReadOnlyDictionary<string, Result<GenericFormattableString>> results)
    {
        builder = builder.IsNotNull(nameof(builder));
        results = results.IsNotNull(nameof(results));

        if (Settings.AddNullChecks)
        {
            var nullCheckStatement = results["ArgumentNullCheck"].Value!;
            if (!string.IsNullOrEmpty(nullCheckStatement))
            {
                builder.AddStringCodeStatements(nullCheckStatement);
            }
        }
    }

    protected TypenameMapping[] GetTypenameMappings(string typeName)
    {
        var typeNameMappings = Settings.TypenameMappings.Where(x => x.SourceTypeName == typeName).ToArray();
        if (typeNameMappings.Length == 0 && typeName.IsCollectionTypeName() && !string.IsNullOrEmpty(typeName.GetCollectionItemType()))
        {
            if (!string.IsNullOrEmpty(typeName.GetCollectionItemType().GetTypeGenericArguments()))
            {
                typeNameMappings = Settings.TypenameMappings.Where(x => x.SourceTypeName == typeName.GetCollectionItemType().WithoutGenerics()).ToArray();
            }
            else
            {
                typeNameMappings = Settings.TypenameMappings.Where(x => x.SourceTypeName == typeName.GetCollectionItemType()).ToArray();
            }
        }

        if (typeNameMappings.Length == 0 && !typeName.IsCollectionTypeName() && !string.IsNullOrEmpty(typeName.GetGenericArguments()))
        {
            typeNameMappings = Settings.TypenameMappings.Where(x => x.SourceTypeName == typeName.WithoutGenerics()).ToArray();
        }

        return typeNameMappings;
    }

    protected static string GetNamespace(string typeName)
    {
        if (typeName.IsCollectionTypeName() && !string.IsNullOrEmpty(typeName.GetCollectionItemType()))
        {
            if (!string.IsNullOrEmpty(typeName.GetCollectionItemType().GetTypeGenericArguments()))
            {
                return typeName.GetCollectionItemType().WithoutGenerics().GetNamespaceWithDefault();
            }
            else
            {
                return typeName.GetCollectionItemType().GetNamespaceWithDefault();
            }
        }

        return typeName.GetNamespaceWithDefault();
    }

    public Domain.Attribute InitializeDelegate(System.Attribute sourceAttribute)
        => Settings.AttributeInitializers
            .Select(x => x(sourceAttribute))
            .FirstOrDefault(x => x is not null)
                ?? throw new NotSupportedException($"Attribute not supported by initializer:");

    public IEnumerable<string> CreateEntityValidationCode()
    {
        var argumentValidationType = Settings.AddValidationCode();

        if (argumentValidationType == ArgumentValidationType.IValidatableObject)
        {
            yield return $"{typeof(Validator).FullName}.{nameof(Validator.ValidateObject)}(this, new {typeof(ValidationContext).FullName}(this, null, null), true);";
        }
        else if (argumentValidationType == ArgumentValidationType.CustomValidationCode)
        {
            yield return "Validate();";
        }
    }

    public IEnumerable<Metadata> GetMappingMetadata(string typeName)
    {
        typeName = typeName.IsNotNull(nameof(typeName)).FixTypeName();

        var typeNameMappings = GetTypenameMappings(typeName);
        if (typeNameMappings.Length > 0)
        {
            return typeNameMappings.SelectMany(x => x.Metadata);
        }

        var ns = GetNamespace(typeName);

        if (!string.IsNullOrEmpty(ns))
        {
            return Settings.NamespaceMappings
                .Where(x => x.SourceNamespace == ns)
                .SelectMany(x => x.Metadata);
        }

        return [];
    }
}

public abstract class MappedContextBase(PipelineSettings settings, IFormatProvider formatProvider) : ContextBase(settings, formatProvider)
{
    protected abstract string NewCollectionTypeName { get; }

    public string MapTypeName(string typeName, string alternateTypeMetadataName)
    {
        typeName = typeName.IsNotNull(nameof(typeName));

        return typeName.MapTypeName(Settings, NewCollectionTypeName, alternateTypeMetadataName);
    }
}

public abstract class ContextBase<TSourceModel>(TSourceModel sourceModel, PipelineSettings settings, IFormatProvider formatProvider) : MappedContextBase(settings, formatProvider)
{
    public TSourceModel SourceModel { get; } = sourceModel.IsNotNull(nameof(sourceModel));

    public IEnumerable<AttributeBuilder> GetAtributes(IReadOnlyCollection<Domain.Attribute> attributes)
    {
        if (!Settings.CopyAttributes)
        {
            return [];
        }

        return attributes
            .Where(x => Settings.CopyAttributePredicate?.Invoke(x) ?? true)
            .Select(x => MapAttribute(x).ToBuilder());
    }

    public Domain.Attribute MapAttribute(Domain.Attribute attribute)
    {
        attribute = attribute.IsNotNull(nameof(attribute));

        return new AttributeBuilder(attribute)
            .WithName(MapTypeName(attribute.Name.FixTypeName(), string.Empty))
            .Build();
    }

    //public Result<GenericFormattableString> GetBuilderPlaceholderProcessorResultForPipelineContext(
    //    string value,
    //    IExpressionEvaluator evaluator,
    //    object context,
    //    IType sourceModel,
    //    IEnumerable<IPipelinePlaceholderProcessor> pipelinePlaceholderProcessors)
    //{
    //    sourceModel = sourceModel.IsNotNull(nameof(sourceModel));
    //    evaluator = evaluator.IsNotNull(nameof(evaluator));
    //    pipelinePlaceholderProcessors = pipelinePlaceholderProcessors.IsNotNull(nameof(pipelinePlaceholderProcessors));

    //    return value switch
    //    {
    //        "NullCheck.Source" => Result.Success<GenericFormattableString>(Settings.AddNullChecks
    //            ? CreateArgumentNullException("source")
    //            : string.Empty),
    //        "BuildersNamespace" => evaluator.Parse(Settings.BuilderNamespaceFormatString, FormatProvider, context),
    //        _ => pipelinePlaceholderProcessors.Select(x => x.Evaluate(value, new PlaceholderSettingsBuilder().WithFormatProvider(FormatProvider), new PipelineContext<IType>(sourceModel), evaluator)).FirstOrDefault(x => x.Status != ResultStatus.Continue)
    //            ?? Result.Continue<GenericFormattableString>()
    //    };
    //}

    //public Result<GenericFormattableString> GetBuilderPlaceholderProcessorResultForParentChildContext(
    //    string value,
    //    IExpressionEvaluator evaluator,
    //    ContextBase context,
    //    IType parentContextModel,
    //    Property childContext,
    //    IType sourceModel,
    //    IEnumerable<IPipelinePlaceholderProcessor> pipelinePlaceholderProcessors)
    //{
    //    sourceModel = sourceModel.IsNotNull(nameof(sourceModel));
    //    evaluator = evaluator.IsNotNull(nameof(evaluator));
    //    context = context.IsNotNull(nameof(context));
    //    parentContextModel = parentContextModel.IsNotNull(nameof(parentContextModel));
    //    pipelinePlaceholderProcessors = pipelinePlaceholderProcessors.IsNotNull(nameof(pipelinePlaceholderProcessors));
    //    childContext = childContext.IsNotNull(nameof(childContext));

    //    // note that for now, we assume that a generic type argument should not be included in argument null checks...
    //    // this might be the case (for example there is a constraint on class), but this is not supported yet
    //    var isGenericArgument = parentContextModel.GenericTypeArguments.Contains(childContext.TypeName);

    //    return value switch
    //    {
    //        "NullCheck.Source.Argument" => Result.Success<GenericFormattableString>(Settings.AddNullChecks && Settings.AddValidationCode() == ArgumentValidationType.None && !childContext.IsNullable && !childContext.IsValueType && !isGenericArgument// only if the source entity does not use validation...
    //            ? $"if (source.{childContext.Name} is not null) "
    //            : string.Empty),
    //        "NullCheck.Argument" => Result.Success<GenericFormattableString>(Settings.AddNullChecks && !childContext.IsValueType && !childContext.IsNullable && !isGenericArgument
    //            ? CreateArgumentNullException(childContext.Name.ToCamelCase(context.FormatProvider.ToCultureInfo()).GetCsharpFriendlyName())
    //            : string.Empty),
    //        "BuildersNamespace" => evaluator.Parse(Settings.BuilderNamespaceFormatString, context.FormatProvider, context),
    //        _ => Default(value, evaluator, childContext, sourceModel, pipelinePlaceholderProcessors)
    //    };

    //    Result<GenericFormattableString> Default(string value, IExpressionEvaluator evaluator, Property childContext, IType sourceModel, IEnumerable<IPipelinePlaceholderProcessor> pipelinePlaceholderProcessors)
    //    {
    //        var pipelinePlaceholderProcessorsArray = pipelinePlaceholderProcessors.ToArray();
    //        return pipelinePlaceholderProcessorsArray.Select(x => x.Evaluate(value, new PlaceholderSettingsBuilder().WithFormatProvider(context.FormatProvider), new PropertyContext(childContext, Settings, context.FormatProvider, MapTypeName(childContext.TypeName, string.Empty), Settings.BuilderNewCollectionTypeName), evaluator)).FirstOrDefault(x => x.Status != ResultStatus.Continue)
    //            ?? pipelinePlaceholderProcessorsArray.Select(x => x.Evaluate(value, new PlaceholderSettingsBuilder().WithFormatProvider(context.FormatProvider), new PipelineContext<IType>(sourceModel), evaluator)).FirstOrDefault(x => x.Status != ResultStatus.Continue)
    //            ?? Result.Continue<GenericFormattableString>();
    //    }
    //}

    public PropertyBuilder CreatePropertyForEntity(Property property, string alternateTypeMetadataName = "")
    {
        property = property.IsNotNull(nameof(property));

        return new PropertyBuilder()
            .WithName(property.Name)
            .WithTypeName(MapTypeName(property.TypeName
                .FixCollectionTypeName(Settings.EntityNewCollectionTypeName)
                .FixNullableTypeName(property), alternateTypeMetadataName))
            .WithIsNullable(property.IsNullable)
            .WithIsValueType(property.IsValueType)
            .AddGenericTypeArguments(property.GenericTypeArguments.Select(x => x.ToBuilder().WithTypeName(MapTypeName(x.TypeName, alternateTypeMetadataName))))
            .AddAttributes(property.Attributes
                .Where(x => Settings.CopyAttributes && (Settings.CopyAttributePredicate?.Invoke(x) ?? true))
                .Select(x => MapAttribute(x).ToBuilder()))
            .WithStatic(property.Static)
            .WithVisibility(property.Visibility)
            .WithParentTypeFullName(property.ParentTypeFullName);
    }

    public async Task<IReadOnlyDictionary<string, Result<GenericFormattableString>>> GetResultsForBuilderCollectionProperties(
        Property property,
        object parentChildContext,
        IExpressionEvaluator evaluator,
        IEnumerable<Result<GenericFormattableString>> enumerableOverloadCode,
        IEnumerable<Result<GenericFormattableString>> arrayOverloadCode)
    {
        property = property.IsNotNull(nameof(property));
        parentChildContext = parentChildContext.IsNotNull(nameof(parentChildContext));
        evaluator = evaluator.IsNotNull(nameof(evaluator));
        enumerableOverloadCode = enumerableOverloadCode.IsNotNull(nameof(enumerableOverloadCode));
        arrayOverloadCode = arrayOverloadCode.IsNotNull(nameof(arrayOverloadCode));

        return await new AsyncResultDictionaryBuilder<GenericFormattableString>()
            .Add(NamedResults.TypeName, () => property.GetBuilderArgumentTypeName(this, parentChildContext, MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName), evaluator))
            .Add(NamedResults.Namespace, evaluator.Parse(Settings.BuilderNamespaceFormatString, FormatProvider, parentChildContext))
            .Add(NamedResults.BuilderName, evaluator.Parse(Settings.BuilderNameFormatString, FormatProvider, parentChildContext))
            .Add("AddMethodName", evaluator.Parse(Settings.AddMethodNameFormatString, FormatProvider, parentChildContext))
            .AddRange("EnumerableOverload.{0}", enumerableOverloadCode)
            .AddRange("ArrayOverload.{0}", arrayOverloadCode)
            .Build()
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyDictionary<string, Result<GenericFormattableString>>> GetResultsForBuilderNonCollectionProperties(
        Property property,
        object parentChildContext,
        IExpressionEvaluator evaluator)
    {
        property = property.IsNotNull(nameof(property));
        parentChildContext = parentChildContext.IsNotNull(nameof(parentChildContext));
        evaluator = evaluator.IsNotNull(nameof(evaluator));

        return await new AsyncResultDictionaryBuilder<GenericFormattableString>()
            .Add(NamedResults.TypeName, () => property.GetBuilderArgumentTypeName(this, parentChildContext, MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName), evaluator))
            .Add(NamedResults.Namespace, evaluator.Parse(Settings.BuilderNamespaceFormatString, FormatProvider, parentChildContext))
            .Add("MethodName", evaluator.Parse(Settings.SetMethodNameFormatString, FormatProvider, parentChildContext))
            .Add(NamedResults.BuilderName, evaluator.Parse(Settings.BuilderNameFormatString, FormatProvider, parentChildContext))
            .Add("ArgumentNullCheck", evaluator.Parse(GetMappingMetadata(property.TypeName).GetStringValue(MetadataNames.CustomBuilderArgumentNullCheckExpression, "{NullCheck.Argument}"), FormatProvider, parentChildContext))
            .Add("BuilderWithExpression", evaluator.Parse(GetMappingMetadata(property.TypeName).GetStringValue(MetadataNames.CustomBuilderWithExpression, "{InstancePrefix()}{$property.Name} = {CsharpFriendlyName(ToCamelCase($property.Name))};"), FormatProvider, parentChildContext))
            .Build()
            .ConfigureAwait(false);
    }

    public string GetMappedTypeName(Type type, MemberInfo declaringType)
    {
        var result = type.GetTypeName(declaringType);

        //HACK for wrong detection of nullability of multiple or nested generic arguments
        var customResult = Settings.TypenameMappings
            .Where(x => x.SourceTypeName == result)
            .SelectMany(x => x.Metadata)
            .GetStringValue(MetadataNames.CustomTypeName);

        if (!string.IsNullOrEmpty(customResult))
        {
            return customResult!;
        }

        return result;
    }

    public ParameterBuilder CreateParameterForBuilder(Property property, string typeName)
    {
        property = property.IsNotNull(nameof(property));
        typeName = typeName.IsNotNull(nameof(typeName));

        return new ParameterBuilder()
            .WithName(property.Name.ToCamelCase(FormatProvider.ToCultureInfo()))
            .WithTypeName(typeName)
            .SetTypeContainerPropertiesFrom(property)
            .WithDefaultValue(GetMappingMetadata(property.TypeName).GetValue<object?>(MetadataNames.CustomBuilderWithDefaultPropertyValue, () => null));
    }
}

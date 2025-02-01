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

    public void AddNullChecks(MethodBuilder builder, Dictionary<string, Result<GenericFormattableString>> results)
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

public abstract class ContextBase<TSourceModel>(TSourceModel sourceModel, PipelineSettings settings, IFormatProvider formatProvider) : MappedContextBase(settings, formatProvider), ITypeNameMapper
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

    public Result<GenericFormattableString> GetBuilderPlaceholderProcessorResultForPipelineContext(string value, IFormattableStringParser formattableStringParser, object context, IType sourceModel, IEnumerable<IPipelinePlaceholderProcessor> pipelinePlaceholderProcessors)
    {
        sourceModel = sourceModel.IsNotNull(nameof(sourceModel));
        formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
        pipelinePlaceholderProcessors = pipelinePlaceholderProcessors.IsNotNull(nameof(pipelinePlaceholderProcessors));

        return value switch
        {
            "NullCheck.Source" => Result.Success<GenericFormattableString>(Settings.AddNullChecks
                ? CreateArgumentNullException("source")
                : string.Empty),
            "BuildersNamespace" => formattableStringParser.Parse(Settings.BuilderNamespaceFormatString, FormatProvider, context),
            _ => pipelinePlaceholderProcessors.Select(x => x.Evaluate(value, FormatProvider, new PipelineContext<IType>(sourceModel), formattableStringParser)).FirstOrDefault(x => x.Status != ResultStatus.Continue)
                ?? Result.Continue<GenericFormattableString>()
        };
    }

    public Result<GenericFormattableString> GetBuilderPlaceholderProcessorResultForParentChildContext(
        string value,
        IFormattableStringParser formattableStringParser,
        ContextBase context,
        IType parentContextModel,
        Property childContext,
        IType sourceModel,
        IEnumerable<IPipelinePlaceholderProcessor> pipelinePlaceholderProcessors)
    {
        sourceModel = sourceModel.IsNotNull(nameof(sourceModel));
        formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
        context = context.IsNotNull(nameof(context));
        parentContextModel = parentContextModel.IsNotNull(nameof(parentContextModel));
        pipelinePlaceholderProcessors = pipelinePlaceholderProcessors.IsNotNull(nameof(pipelinePlaceholderProcessors));
        childContext = childContext.IsNotNull(nameof(childContext));

        // note that for now, we assume that a generic type argument should not be included in argument null checks...
        // this might be the case (for example there is a constraint on class), but this is not supported yet
        var isGenericArgument = parentContextModel.GenericTypeArguments.Contains(childContext.TypeName);

        return value switch
        {
            "NullCheck.Source.Argument" => Result.Success<GenericFormattableString>(Settings.AddNullChecks && Settings.AddValidationCode() == ArgumentValidationType.None && !childContext.IsNullable && !childContext.IsValueType && !isGenericArgument// only if the source entity does not use validation...
                ? $"if (source.{childContext.Name} is not null) "
                : string.Empty),
            "NullCheck.Argument" => Result.Success<GenericFormattableString>(Settings.AddNullChecks && !childContext.IsValueType && !childContext.IsNullable && !isGenericArgument
                ? CreateArgumentNullException(childContext.Name.ToCamelCase(context.FormatProvider.ToCultureInfo()).GetCsharpFriendlyName())
                : string.Empty),
            "BuildersNamespace" => formattableStringParser.Parse(Settings.BuilderNamespaceFormatString, context.FormatProvider, context),
            _ => Default(value, formattableStringParser, childContext, sourceModel, pipelinePlaceholderProcessors)
        };

        Result<GenericFormattableString> Default(string value, IFormattableStringParser formattableStringParser, Property childContext, IType sourceModel, IEnumerable<IPipelinePlaceholderProcessor> pipelinePlaceholderProcessors)
        {
            var pipelinePlaceholderProcessorsArray = pipelinePlaceholderProcessors.ToArray();
            return pipelinePlaceholderProcessorsArray.Select(x => x.Evaluate(value, context.FormatProvider, new PropertyContext(childContext, Settings, context.FormatProvider, MapTypeName(childContext.TypeName, string.Empty), Settings.BuilderNewCollectionTypeName), formattableStringParser)).FirstOrDefault(x => x.Status != ResultStatus.Continue)
                ?? pipelinePlaceholderProcessorsArray.Select(x => x.Evaluate(value, context.FormatProvider, new PipelineContext<IType>(sourceModel), formattableStringParser)).FirstOrDefault(x => x.Status != ResultStatus.Continue)
                ?? Result.Continue<GenericFormattableString>();
        }
    }

    public PropertyBuilder CreatePropertyForEntity(Property property)
    {
        property = property.IsNotNull(nameof(property));

        return new PropertyBuilder()
            .WithName(property.Name)
            .WithTypeName(MapTypeName(property.TypeName
                .FixCollectionTypeName(Settings.EntityNewCollectionTypeName)
                .FixNullableTypeName(property), string.Empty))
            .WithIsNullable(property.IsNullable)
            .WithIsValueType(property.IsValueType)
            .AddGenericTypeArguments(property.GenericTypeArguments)
            .AddAttributes(property.Attributes
                .Where(x => Settings.CopyAttributes && (Settings.CopyAttributePredicate?.Invoke(x) ?? true))
                .Select(x => MapAttribute(x).ToBuilder()))
            .WithStatic(property.Static)
            .WithVisibility(property.Visibility)
            .WithParentTypeFullName(property.ParentTypeFullName);
    }

    public Dictionary<string, Result<GenericFormattableString>> GetResultsForBuilderCollectionProperties(
        Property property,
        object parentChildContext,
        IFormattableStringParser formattableStringParser,
        IEnumerable<Result<GenericFormattableString>> enumerableOverloadCode,
        IEnumerable<Result<GenericFormattableString>> arrayOverloadCode)
    {
        property = property.IsNotNull(nameof(property));
        parentChildContext = parentChildContext.IsNotNull(nameof(parentChildContext));
        formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
        enumerableOverloadCode = enumerableOverloadCode.IsNotNull(nameof(enumerableOverloadCode));
        arrayOverloadCode = arrayOverloadCode.IsNotNull(nameof(arrayOverloadCode));

        return new ResultDictionaryBuilder<GenericFormattableString>()
            .Add(NamedResults.TypeName, () => property.GetBuilderArgumentTypeName(this, parentChildContext, MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName), formattableStringParser))
            .Add(NamedResults.Namespace, () => formattableStringParser.Parse(Settings.BuilderNamespaceFormatString, FormatProvider, parentChildContext))
            .Add(NamedResults.BuilderName, () => formattableStringParser.Parse(Settings.BuilderNameFormatString, FormatProvider, parentChildContext))
            .Add("AddMethodName", () => formattableStringParser.Parse(Settings.AddMethodNameFormatString, FormatProvider, parentChildContext))
            .AddRange("EnumerableOverload.{0}", () => enumerableOverloadCode)
            .AddRange("ArrayOverload.{0}", () => arrayOverloadCode)
            .Build();
    }

    public Dictionary<string, Result<GenericFormattableString>> GetResultsForBuilderNonCollectionProperties(
        Property property,
        object parentChildContext,
        IFormattableStringParser formattableStringParser)
    {
        property = property.IsNotNull(nameof(property));
        parentChildContext = parentChildContext.IsNotNull(nameof(parentChildContext));
        formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

        return new ResultDictionaryBuilder<GenericFormattableString>()
            .Add(NamedResults.TypeName, () => property.GetBuilderArgumentTypeName(this, parentChildContext, MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName), formattableStringParser))
            .Add(NamedResults.Namespace, () => formattableStringParser.Parse(Settings.BuilderNamespaceFormatString, FormatProvider, parentChildContext))
            .Add("MethodName", () => formattableStringParser.Parse(Settings.SetMethodNameFormatString, FormatProvider, parentChildContext))
            .Add(NamedResults.BuilderName, () => formattableStringParser.Parse(Settings.BuilderNameFormatString, FormatProvider, parentChildContext))
            .Add("ArgumentNullCheck", () => formattableStringParser.Parse(GetMappingMetadata(property.TypeName).GetStringValue(MetadataNames.CustomBuilderArgumentNullCheckExpression, "{NullCheck.Argument}"), FormatProvider, parentChildContext))
            .Add("BuilderWithExpression", () => formattableStringParser.Parse(GetMappingMetadata(property.TypeName).GetStringValue(MetadataNames.CustomBuilderWithExpression, "{InstancePrefix()}{$property.Name} = {CsharpFriendlyName(ToCamelCase($property.Name))};"), FormatProvider, parentChildContext))
            .Build();
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

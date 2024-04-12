namespace ClassFramework.Pipelines;

public abstract class ContextBase
{
    public PipelineSettings Settings { get; }
    public IFormatProvider FormatProvider { get; }

    protected ContextBase(PipelineSettings settings, IFormatProvider formatProvider)
    {
        Settings = settings.IsNotNull(nameof(settings));
        FormatProvider = formatProvider.IsNotNull(nameof(formatProvider));
    }
}

public abstract class ContextBase<TModel> : ContextBase
{
    protected ContextBase(TModel sourceModel, PipelineSettings settings, IFormatProvider formatProvider) : base(settings, formatProvider)
    {
        SourceModel = sourceModel.IsNotNull(nameof(sourceModel));
    }

    protected abstract string NewCollectionTypeName { get; }

    public TModel SourceModel { get; }

    public string CreateArgumentNullException(string argumentName)
    {
        if (Settings.UseExceptionThrowIfNull)
        {
            return $"{typeof(ArgumentNullException).FullName}.ThrowIfNull({argumentName});";
        }

        return $"if ({argumentName} is null) throw new {typeof(ArgumentNullException).FullName}(nameof({argumentName}));";
    }

    public string MapTypeName(string typeName, string alternateTypeMetadataName = "")
    {
        typeName = typeName.IsNotNull(nameof(typeName));

        return typeName.MapTypeName(Settings, NewCollectionTypeName, alternateTypeMetadataName);
    }

    public string MapNamespace(string? ns)
        => ns.MapNamespace(Settings);

    public IEnumerable<AttributeBuilder> GetAtributes(IReadOnlyCollection<Domain.Attribute> attributes)
    {
        if (!Settings.CopyAttributes)
        {
            return Enumerable.Empty<AttributeBuilder>();
        }

        return attributes
            .Where(x => Settings.CopyAttributePredicate?.Invoke(x) ?? true)
            .Select(x => MapAttribute(x).ToBuilder());
    }

    public Domain.Attribute MapAttribute(Domain.Attribute attribute)
    {
        attribute = attribute.IsNotNull(nameof(attribute));

        return new AttributeBuilder(attribute)
            .WithName(MapTypeName(attribute.Name.FixTypeName()))
            .Build();
    }

    public Result<string> GetBuilderPlaceholderProcessorResultForPipelineContext(string value, IFormattableStringParser formattableStringParser, object context, IType sourceModel, IEnumerable<IPipelinePlaceholderProcessor> pipelinePlaceholderProcessors)
    {
        sourceModel = sourceModel.IsNotNull(nameof(sourceModel));
        formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
        pipelinePlaceholderProcessors = pipelinePlaceholderProcessors.IsNotNull(nameof(pipelinePlaceholderProcessors));

        return value switch
        {
            "NullCheck.Source" => Result.Success(Settings.AddNullChecks
                ? CreateArgumentNullException("source")
                : string.Empty),
            "BuildersNamespace" => formattableStringParser.Parse(Settings.BuilderNamespaceFormatString, FormatProvider, context),
            _ => pipelinePlaceholderProcessors.Select(x => x.Process(value, FormatProvider, new PipelineContext<IType>(sourceModel), formattableStringParser)).FirstOrDefault(x => x.Status != ResultStatus.Continue)
                ?? Result.Continue<string>()
        };
    }

    public Result<string> GetBuilderPlaceholderProcessorResultForParentChildContext(
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
            "NullCheck.Source.Argument" => Result.Success(Settings.AddNullChecks && Settings.AddValidationCode() == ArgumentValidationType.None && !childContext.IsNullable && !childContext.IsValueType && !isGenericArgument// only if the source entity does not use validation...
                ? $"if (source.{childContext.Name} is not null) "
                : string.Empty),
            "NullCheck.Argument" => Result.Success(Settings.AddNullChecks && !childContext.IsValueType && !childContext.IsNullable && !isGenericArgument
                ? CreateArgumentNullException(childContext.Name.ToPascalCase(context.FormatProvider.ToCultureInfo()).GetCsharpFriendlyName())
                : string.Empty),
            "NullableRequiredSuffix" => Result.Success(!Settings.AddNullChecks && !childContext.IsValueType && childContext.IsNullable && Settings.EnableNullableReferenceTypes && !isGenericArgument
                ? "!"
                : string.Empty),
            "NullableSuffix" => Result.Success(childContext.IsNullable && (childContext.IsValueType || Settings.EnableNullableReferenceTypes)
                ? "?"
                : string.Empty),
            "BuildersNamespace" => formattableStringParser.Parse(Settings.BuilderNamespaceFormatString, context.FormatProvider, context),
            _ => Default(value, formattableStringParser, childContext, sourceModel, pipelinePlaceholderProcessors)
        };

        Result<string> Default(string value, IFormattableStringParser formattableStringParser, Property childContext, IType sourceModel, IEnumerable<IPipelinePlaceholderProcessor> pipelinePlaceholderProcessors)
        {
            var pipelinePlaceholderProcessorsArray = pipelinePlaceholderProcessors.ToArray();
            return pipelinePlaceholderProcessorsArray.Select(x => x.Process(value, context.FormatProvider, new PropertyContext(childContext, Settings, context.FormatProvider, MapTypeName(childContext.TypeName), Settings.BuilderNewCollectionTypeName), formattableStringParser)).FirstOrDefault(x => x.Status != ResultStatus.Continue)
                ?? pipelinePlaceholderProcessorsArray.Select(x => x.Process(value, context.FormatProvider, new PipelineContext<IType>(sourceModel), formattableStringParser)).FirstOrDefault(x => x.Status != ResultStatus.Continue)
                ?? Result.Continue<string>();
        }
    }

    public PropertyBuilder CreatePropertyForEntity(Property property)
    {
        property = property.IsNotNull(nameof(property));

        return new PropertyBuilder()
            .WithName(property.Name)
            .WithTypeName(MapTypeName(property.TypeName
                .FixCollectionTypeName(Settings.EntityNewCollectionTypeName)
                .FixNullableTypeName(property)))
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

    public Domain.Attribute InitializeDelegate(System.Attribute sourceAttribute)
        => Settings.AttributeInitializers
            .Select(x => x(sourceAttribute))
            .FirstOrDefault(x => x is not null)
                ?? throw new NotSupportedException($"Attribute not supported by initializer:");

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

        return Enumerable.Empty<Metadata>();
    }

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

    public NamedResult<Result<string>>[] GetResultsForBuilderCollectionProperties(
        Property property,
        object parentChildContext,
        IFormattableStringParser formattableStringParser,
        IEnumerable<Result<string>> enumerableOverloadCode,
        IEnumerable<Result<string>> arrayOverloadCode)
    {
        property = property.IsNotNull(nameof(property));
        parentChildContext = parentChildContext.IsNotNull(nameof(parentChildContext));
        formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
        enumerableOverloadCode = enumerableOverloadCode.IsNotNull(nameof(enumerableOverloadCode));
        arrayOverloadCode = arrayOverloadCode.IsNotNull(nameof(arrayOverloadCode));

        var resultSetBuilder = new NamedResultSetBuilder<string>();
        resultSetBuilder.Add(NamedResults.TypeName, () => property.GetBuilderArgumentTypeName(this, parentChildContext, MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName), formattableStringParser));
        resultSetBuilder.Add(NamedResults.Namespace, () => formattableStringParser.Parse(Settings.BuilderNamespaceFormatString, FormatProvider, parentChildContext));
        resultSetBuilder.Add(NamedResults.BuilderName, () => formattableStringParser.Parse(Settings.BuilderNameFormatString, FormatProvider, parentChildContext));
        resultSetBuilder.Add("AddMethodName", () => formattableStringParser.Parse(Settings.AddMethodNameFormatString, FormatProvider, parentChildContext));
        resultSetBuilder.AddRange("EnumerableOverload", () => enumerableOverloadCode);
        resultSetBuilder.AddRange("ArrayOverload", () => arrayOverloadCode);
        
        return resultSetBuilder.Build();
    }

    public NamedResult<Result<string>>[] GetResultsForBuilderNonCollectionProperties(
        Property property,
        object parentChildContext,
        IFormattableStringParser formattableStringParser)
    {
        property = property.IsNotNull(nameof(property));
        parentChildContext = parentChildContext.IsNotNull(nameof(parentChildContext));
        formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

        var resultSetBuilder = new NamedResultSetBuilder<string>();
        resultSetBuilder.Add(NamedResults.TypeName, () => property.GetBuilderArgumentTypeName(this, parentChildContext, MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName), formattableStringParser));
        resultSetBuilder.Add(NamedResults.Namespace, () => formattableStringParser.Parse(Settings.BuilderNamespaceFormatString, FormatProvider, parentChildContext));
        resultSetBuilder.Add("MethodName", () => formattableStringParser.Parse(Settings.SetMethodNameFormatString, FormatProvider, parentChildContext));
        resultSetBuilder.Add(NamedResults.BuilderName, () => formattableStringParser.Parse(Settings.BuilderNameFormatString, FormatProvider, parentChildContext));
        resultSetBuilder.Add("ArgumentNullCheck", () => formattableStringParser.Parse(GetMappingMetadata(property.TypeName).GetStringValue(MetadataNames.CustomBuilderArgumentNullCheckExpression, "{NullCheck.Argument}"), FormatProvider, parentChildContext));
        resultSetBuilder.Add("BuilderWithExpression", () => formattableStringParser.Parse(GetMappingMetadata(property.TypeName).GetStringValue(MetadataNames.CustomBuilderWithExpression, "{InstancePrefix}{Name} = {NamePascalCsharpFriendlyName};"), FormatProvider, parentChildContext));

        return resultSetBuilder.Build();
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
            .WithName(property.Name.ToPascalCase(FormatProvider.ToCultureInfo()))
            .WithTypeName(typeName)
            .SetTypeContainerPropertiesFrom(property)
            .WithDefaultValue(GetMappingMetadata(property.TypeName).GetValue<object?>(MetadataNames.CustomBuilderWithDefaultPropertyValue, () => null));
    }

    public void AddNullChecks(MethodBuilder builder, NamedResult<Result<string>>[] results)
    {
        builder = builder.IsNotNull(nameof(builder));
        results = results.IsNotNull(nameof(results));

        if (Settings.AddNullChecks)
        {
            var nullCheckStatement = results.First(x => x.Name == "ArgumentNullCheck").Result.Value!;
            if (!string.IsNullOrEmpty(nullCheckStatement))
            {
                builder.AddStringCodeStatements(nullCheckStatement);
            }
        }
    }

    private TypenameMapping[] GetTypenameMappings(string typeName)
    {
        var typeNameMappings = Settings.TypenameMappings.Where(x => x.SourceTypeName == typeName).ToArray();
        if (typeNameMappings.Length == 0 && typeName.IsCollectionTypeName() && !string.IsNullOrEmpty(typeName.GetCollectionItemType()))
        {
            if (!string.IsNullOrEmpty(typeName.GetCollectionItemType().GetGenericArguments()))
            {
                typeNameMappings = Settings.TypenameMappings.Where(x => x.SourceTypeName == typeName.GetCollectionItemType().WithoutProcessedGenerics()).ToArray();
            }
            else
            {
                typeNameMappings = Settings.TypenameMappings.Where(x => x.SourceTypeName == typeName.GetCollectionItemType()).ToArray();
            }
        }

        if (typeNameMappings.Length == 0 && !typeName.IsCollectionTypeName() && !string.IsNullOrEmpty(typeName.GetProcessedGenericArguments()))
        {
            typeNameMappings = Settings.TypenameMappings.Where(x => x.SourceTypeName == typeName.WithoutProcessedGenerics()).ToArray();
        }

        return typeNameMappings;
    }

    private static string GetNamespace(string typeName)
    {
        if (typeName.IsCollectionTypeName() && !string.IsNullOrEmpty(typeName.GetCollectionItemType()))
        {
            if (!string.IsNullOrEmpty(typeName.GetCollectionItemType().GetGenericArguments()))
            {
                return typeName.GetCollectionItemType().WithoutProcessedGenerics().GetNamespaceWithDefault();
            }
            else
            {
                return typeName.GetCollectionItemType().GetNamespaceWithDefault();
            }
        }

        return typeName.GetNamespaceWithDefault();
    }
}

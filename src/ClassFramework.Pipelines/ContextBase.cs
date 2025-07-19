namespace ClassFramework.Pipelines;

public abstract class ContextBase(PipelineSettings settings, IFormatProvider formatProvider, CancellationToken cancellationToken)
{
    public PipelineSettings Settings { get; } = settings.IsNotNull(nameof(settings));
    public IFormatProvider FormatProvider { get; } = formatProvider.IsNotNull(nameof(formatProvider));
    public CancellationToken CancellationToken { get; } = cancellationToken.IsNotNull(nameof(cancellationToken));

    public const string DefaultBuilderName = "{NoGenerics(ClassName(property.TypeName))}Builder";

    public string NullCheck => Settings.UsePatternMatchingForNullChecks
        ? "is null"
        : "== null";

    public string NotNullCheck => Settings.UsePatternMatchingForNullChecks
        ? "is not null"
        : "!= null";

    public string CollectionTypeName
        => Settings.CollectionTypeName.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics());

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
            var nullCheckStatement = results.GetValue("ArgumentNullCheck");
            if (!string.IsNullOrEmpty(nullCheckStatement))
            {
                builder.AddCodeStatements(nullCheckStatement);
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

public abstract class MappedContextBase(PipelineSettings settings, IFormatProvider formatProvider, CancellationToken cancellationToken) : ContextBase(settings, formatProvider, cancellationToken)
{
    protected abstract string NewCollectionTypeName { get; }

    public string MapTypeName(string typeName)
        => MapTypeName(typeName, string.Empty);

    public string MapTypeName(string typeName, string alternateTypeMetadataName)
    {
        typeName = typeName.IsNotNull(nameof(typeName));

        return typeName.MapTypeName(Settings, NewCollectionTypeName, alternateTypeMetadataName);
    }
}

public abstract class ContextBase<TSourceModel>(TSourceModel sourceModel, PipelineSettings settings, IFormatProvider formatProvider, CancellationToken cancellationToken) : MappedContextBase(settings, formatProvider, cancellationToken)
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
            .WithName(MapTypeName(attribute.Name.FixTypeName()))
            .Build();
    }

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
            .Add(ResultNames.TypeName, property.GetBuilderArgumentTypeNameAsync(this, parentChildContext, MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName), evaluator, CancellationToken))
            .Add(ResultNames.Namespace, evaluator.EvaluateInterpolatedStringAsync(Settings.BuilderNamespaceFormatString, FormatProvider, parentChildContext, CancellationToken))
            .Add(ResultNames.BuilderName, evaluator.EvaluateInterpolatedStringAsync(Settings.BuilderNameFormatString, FormatProvider, parentChildContext, CancellationToken))
            .Add(ResultNames.AddMethodName, evaluator.EvaluateInterpolatedStringAsync(Settings.AddMethodNameFormatString, FormatProvider, parentChildContext, CancellationToken))
            .Add(ResultNames.NonLazyTypeName, property.GetBuilderArgumentTypeNameAsync(this, parentChildContext, MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName), evaluator, true, CancellationToken))
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
            .Add(ResultNames.TypeName, property.GetBuilderArgumentTypeNameAsync(this, parentChildContext, MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName), evaluator, CancellationToken))
            .Add(ResultNames.NonLazyTypeName, property.GetBuilderArgumentTypeNameAsync(this, parentChildContext, MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName), evaluator, true, CancellationToken))
            .Add(ResultNames.Namespace, evaluator.EvaluateInterpolatedStringAsync(Settings.BuilderNamespaceFormatString, FormatProvider, parentChildContext, CancellationToken))
            .Add("MethodName", evaluator.EvaluateInterpolatedStringAsync(Settings.SetMethodNameFormatString, FormatProvider, parentChildContext, CancellationToken))
            .Add(ResultNames.BuilderName, evaluator.EvaluateInterpolatedStringAsync(Settings.BuilderNameFormatString, FormatProvider, parentChildContext, CancellationToken))
            .Add("ArgumentNullCheck", evaluator.EvaluateInterpolatedStringAsync(GetMappingMetadata(property.TypeName).GetStringValue(MetadataNames.CustomBuilderArgumentNullCheckExpression, "{ArgumentNullCheck()}"), FormatProvider, parentChildContext, CancellationToken))
            .Add(ResultNames.BuilderWithExpression, evaluator.EvaluateInterpolatedStringAsync(GetMappingMetadata(property.TypeName).GetStringValue(MetadataNames.CustomBuilderWithExpression, "{InstancePrefix()}{property.Name} = {CsharpFriendlyName(property.Name.ToCamelCase())};"), FormatProvider, parentChildContext, CancellationToken))
            .Add(ResultNames.BuilderNonLazyWithExpression, evaluator.EvaluateInterpolatedStringAsync(GetMappingMetadata(property.TypeName).GetStringValue(MetadataNames.CustomBuilderWithExpression, "{InstancePrefix()}{property.Name} = {property.BuilderFuncPrefix}{CsharpFriendlyName(property.Name.ToCamelCase())}{property.BuilderFuncSuffix};"), FormatProvider, parentChildContext, CancellationToken))
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

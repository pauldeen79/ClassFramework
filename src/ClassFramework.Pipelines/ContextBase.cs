namespace ClassFramework.Pipelines;

public abstract class ContextBase<TModel>
{
    protected ContextBase(TModel sourceModel, PipelineSettings settings, IFormatProvider formatProvider)
    {
        SourceModel = sourceModel.IsNotNull(nameof(sourceModel));
        Settings = settings.IsNotNull(nameof(settings));
        FormatProvider = formatProvider.IsNotNull(nameof(formatProvider));
    }

    protected abstract string NewCollectionTypeName { get; }

    public TModel SourceModel { get; }
    public PipelineSettings Settings { get; }
    public IFormatProvider FormatProvider { get; }

    public string CreateArgumentNullException(string argumentName)
    {
        if (Settings.UseExceptionThrowIfNull)
        {
            return $"{typeof(ArgumentNullException).FullName}.ThrowIfNull({argumentName});";
        }

        return $"if ({argumentName} is null) throw new {typeof(ArgumentNullException).FullName}(nameof({argumentName}));";
    }

    public string MapTypeName(string typeName, Func<string, PipelineSettings, string, string>? innerDelegate = null)
        => typeName.IsNotNull(nameof(typeName)).MapTypeName(Settings, NewCollectionTypeName, innerDelegate);

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

    public Result<IConcreteTypeBuilder> SetEntityName(IType sourceModel, IConcreteTypeBuilder model, IFormattableStringParser formattableStringParser, object context)
    {
        var resultSetBuilder = new NamedResultSetBuilder<string>();
        resultSetBuilder.Add("Name", () => formattableStringParser.Parse(Settings.EntityNameFormatString, FormatProvider, context));
        resultSetBuilder.Add("Namespace", () => sourceModel.Metadata.WithMappingMetadata(sourceModel.GetFullName().GetCollectionItemType().WhenNullOrEmpty(sourceModel.GetFullName), Settings).GetStringResult(MetadataNames.CustomEntityNamespace, () => formattableStringParser.Parse(Settings.EntityNamespaceFormatString, FormatProvider, context)));
        var results = resultSetBuilder.Build();

        var error = Array.Find(results, x => !x.Result.IsSuccessful());
        if (error is not null)
        {
            // Error in formattable string parsing
            return Result.FromExistingResult<IConcreteTypeBuilder>(error.Result);
        }

        model
            .WithName(results.First(x => x.Name == "Name").Result.Value!)
            .WithNamespace(MapNamespace(results.First(x => x.Name == "Namespace").Result.Value!));

        return Result.Continue<IConcreteTypeBuilder>();
    }

    public Result<string> GetBuilderPlaceholderProcessorResultForPipelineContext(string value, IFormatProvider formatProvider, IFormattableStringParser formattableStringParser, object context, IType sourceModel, IEnumerable<IPipelinePlaceholderProcessor> pipelinePlaceholderProcessors)
    {
        sourceModel = sourceModel.IsNotNull(nameof(sourceModel));
        formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
        pipelinePlaceholderProcessors = pipelinePlaceholderProcessors.IsNotNull(nameof(pipelinePlaceholderProcessors));

        return value switch
        {
            "NullCheck.Source" => Result.Success(Settings.AddNullChecks
                ? CreateArgumentNullException("source")
                : string.Empty),
            "BuildersNamespace" => formattableStringParser.Parse(Settings.BuilderNamespaceFormatString, formatProvider, context),
            _ => pipelinePlaceholderProcessors.Select(x => x.Process(value, formatProvider, new PipelineContext<IType>(sourceModel), formattableStringParser)).FirstOrDefault(x => x.Status != ResultStatus.Continue)
                ?? Result.Continue<string>()
        };
    }

    public Result<string> GetBuilderPlaceholderProcessorResultForParentChildContext(string value, IFormatProvider formatProvider, IFormattableStringParser formattableStringParser, object context, Property childContext, IType sourceModel, IEnumerable<IPipelinePlaceholderProcessor> pipelinePlaceholderProcessors)
    {
        sourceModel = sourceModel.IsNotNull(nameof(sourceModel));
        formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
        pipelinePlaceholderProcessors = pipelinePlaceholderProcessors.IsNotNull(nameof(pipelinePlaceholderProcessors));
        childContext = childContext.IsNotNull(nameof(childContext));

        return value switch
        {
            "NullCheck.Source.Argument" => Result.Success(Settings.AddNullChecks && Settings.AddValidationCode == ArgumentValidationType.None && !childContext.IsNullable && !childContext.IsValueType // only if the source entity does not use validation...
                ? $"if (source.{childContext.Name} is not null) "
                : string.Empty),
            "NullCheck.Argument" => Result.Success(Settings.AddNullChecks && !childContext.IsValueType && !childContext.IsNullable
                ? CreateArgumentNullException(childContext.Name.ToPascalCase(formatProvider.ToCultureInfo()).GetCsharpFriendlyName())
                : string.Empty),
            "NullableRequiredSuffix" => Result.Success(!Settings.AddNullChecks && !childContext.IsValueType && childContext.IsNullable && Settings.EnableNullableReferenceTypes
                ? "!"
                : string.Empty),
            "NullableSuffix" => Result.Success(childContext.IsNullable && (childContext.IsValueType || Settings.EnableNullableReferenceTypes)
                ? "?"
                : string.Empty),
            "BuildersNamespace" => formattableStringParser.Parse(Settings.BuilderNamespaceFormatString, formatProvider, context),
            _ => Default(value, formatProvider, formattableStringParser, childContext, sourceModel, pipelinePlaceholderProcessors)
        };

        Result<string> Default(string value, IFormatProvider formatProvider, IFormattableStringParser formattableStringParser, Property childContext, IType sourceModel, IEnumerable<IPipelinePlaceholderProcessor> pipelinePlaceholderProcessors)
        {
            var pipelinePlaceholderProcessorsArray = pipelinePlaceholderProcessors.ToArray();
            return pipelinePlaceholderProcessorsArray.Select(x => x.Process(value, formatProvider, new PropertyContext(childContext, Settings, formatProvider, MapTypeName(childContext.TypeName), Settings.BuilderNewCollectionTypeName), formattableStringParser)).FirstOrDefault(x => x.Status != ResultStatus.Continue)
                ?? pipelinePlaceholderProcessors.Select(x => x.Process(value, formatProvider, new PipelineContext<IType>(sourceModel), formattableStringParser)).FirstOrDefault(x => x.Status != ResultStatus.Continue)
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
            .AddAttributes(property.Attributes
                .Where(x => Settings.CopyAttributes && (Settings.CopyAttributePredicate?.Invoke(x) ?? true))
                .Select(x => MapAttribute(x).ToBuilder()))
            .WithStatic(property.Static)
            .WithIsNullable(property.IsNullable)
            .WithIsValueType(property.IsValueType)
            .WithVisibility(property.Visibility)
            .WithParentTypeFullName(property.ParentTypeFullName)
            .AddMetadata(property.Metadata.Select(x => x.ToBuilder()));
    }
}

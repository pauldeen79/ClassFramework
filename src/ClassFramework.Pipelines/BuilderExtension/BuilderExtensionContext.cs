namespace ClassFramework.Pipelines.BuilderExtension;

public class BuilderExtensionContext(TypeBase sourceModel, PipelineSettings settings, IFormatProvider formatProvider, CancellationToken cancellationToken) : ContextBase<TypeBase>(sourceModel, settings, formatProvider, cancellationToken)
{
    private const string Instance = "instance";

    protected override string NewCollectionTypeName => Settings.BuilderNewCollectionTypeName;

    public IEnumerable<Property> GetSourceProperties()
        => SourceModel.Properties.Where(x => SourceModel.IsMemberValidForBuilderClass(x, Settings));

    public ClassBuilder Builder { get; } = new();

    public string GetReturnTypeForFluentMethod(string builderNamespace, string builderName)
        => $"{builderNamespace.AppendWhenNotNullOrEmpty(".")}{builderName}{SourceModel.GetGenericTypeArgumentsString()}";

    public IEnumerable<MethodBuilder> GetFluentMethodsForCollectionProperty(Property property, IReadOnlyDictionary<string, Result<GenericFormattableString>> results, string returnType, string typeNameKey, string enumerableOverloadKeyPrefix, string arrayOverloadKeyPrefix)
    {
        yield return new MethodBuilder()
            .WithName(results.GetValue(ResultNames.AddMethodName))
            .WithReturnTypeName("T")
            .WithStatic()
            .WithExtensionMethod()
            .AddGenericTypeArguments("T")
            .AddGenericTypeArgumentConstraints($"where T : {returnType}")
            .AddParameter(Instance, "T")
            .AddParameters(CreateParameterForBuilder(property, results.GetValue(typeNameKey).ToString().FixCollectionTypeName(typeof(IEnumerable<>).WithoutGenerics())))
            .AddCodeStatements(results.Where(x => x.Key.StartsWith(enumerableOverloadKeyPrefix)).Select(x => x.Value.Value!.ToString()));

        yield return new MethodBuilder()
            .WithName(results.GetValue(ResultNames.AddMethodName))
            .WithReturnTypeName("T")
            .WithStatic()
            .WithExtensionMethod()
            .AddGenericTypeArguments("T")
            .AddGenericTypeArgumentConstraints($"where T : {returnType}")
            .AddParameter(Instance, "T")
            .AddParameters(CreateParameterForBuilder(property, results.GetValue(typeNameKey).ToString().FixTypeName().ConvertTypeNameToArray()).WithIsParamArray())
            .AddCodeStatements(results.Where(x => x.Key.StartsWith(arrayOverloadKeyPrefix)).Select(x => x.Value.Value!.ToString()));
    }

    public IEnumerable<MethodBuilder> GetFluentMethodsForNonCollectionProperty(Property property, IReadOnlyDictionary<string, Result<GenericFormattableString>> results, string returnType, string typeNameKey, string expressionKey)
    {
        yield return new MethodBuilder()
            .WithName(results.GetValue("MethodName"))
            .WithReturnTypeName("T")
            .WithStatic()
            .WithExtensionMethod()
            .AddGenericTypeArguments("T")
            .AddGenericTypeArgumentConstraints($"where T : {returnType}")
            .AddParameter("instance", "T")
            .AddParameters(CreateParameterForBuilder(property, results.GetValue(typeNameKey)))
            .Chain(method => AddNullChecks(method, results))
            .AddCodeStatements
            (
                results.GetValue(expressionKey),
                "return instance;"
            );
    }
}

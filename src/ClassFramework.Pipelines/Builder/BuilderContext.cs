namespace ClassFramework.Pipelines.Builder;

public class BuilderContext(TypeBase sourceModel, PipelineSettings settings, IFormatProvider formatProvider, CancellationToken cancellationToken) : ContextBase<TypeBase>(sourceModel, settings, formatProvider, cancellationToken)
{
    public IEnumerable<Property> GetSourceProperties()
        => SourceModel.Properties.Where(x => SourceModel.IsMemberValidForBuilderClass(x, Settings));

    public bool IsBuilderForAbstractEntity => Settings.EnableInheritance && (Settings.BaseClass is null || Settings.IsAbstract);
    public bool IsBuilderForOverrideEntity => Settings.EnableInheritance && Settings.BaseClass is not null;
    public bool IsAbstractBuilder => Settings.EnableBuilderInheritance && (Settings.BaseClass is null || Settings.IsAbstract) && !Settings.IsForAbstractBuilder;

    protected override string NewCollectionTypeName => Settings.BuilderNewCollectionTypeName;

    public string[] CreatePragmaWarningDisableStatementsForBuildMethod()
        => NeedsPragmasForBuildMethod()
            ?
            [
                "#pragma warning disable CS8604 // Possible null reference argument.",
                "#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.",
            ]
            : [];

    public string[] CreatePragmaWarningRestoreStatementsForBuildMethod()
        => NeedsPragmasForBuildMethod()
            ?
            [
                "#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.",
                "#pragma warning restore CS8604 // Possible null reference argument.",
            ]
            : [];

    public bool HasBackingFields()
        => !(IsAbstractBuilder || !Settings.AddNullChecks)
        || Settings.AddBackingFields
        || Settings.CreateAsObservable;

    public bool IsValidForFluentMethod(Property property)
    {
        property = property.IsNotNull(nameof(property));

        if (string.IsNullOrEmpty(property.ParentTypeFullName))
        {
            return true;
        }

        return Settings.SkipNamespacesOnFluentBuilderMethods.Count == 0
            || !Settings.SkipNamespacesOnFluentBuilderMethods.Contains(property.ParentTypeFullName.GetNamespaceWithDefault());
    }

    public string ReturnValueStatementForFluentMethod => $"return {ReturnValue};";

    public string BuildReturnTypeName
    {
        get
        {
            if (IsBuilderForAbstractEntity || IsBuilderForOverrideEntity)
            {
                return MapTypeName(SourceModel.GetFullName());
            }

            return MapTypeName(SourceModel.Interfaces.FirstOrDefault(x => x.GetClassName() == $"I{SourceModel.Name}") ?? SourceModel.GetFullName());
        }
    }

    public string GetReturnTypeForFluentMethod(string builderNamespace, string builderName)
        => IsBuilderForAbstractEntity
            ? $"TBuilder{SourceModel.GetGenericTypeArgumentsString()}"
            : $"{builderNamespace.AppendWhenNotNullOrEmpty(".")}{builderName}{SourceModel.GetGenericTypeArgumentsString()}";

    public async Task<Result<T>[]> GetInterfaceResultsAsync<T>(
        Func<string, GenericFormattableString, T> namespaceTransformation,
        Func<string, T> noNamespaceTransformation,
        IExpressionEvaluator evaluator,
        bool includeNonAbstractionNamespaces,
        CancellationToken token)
    {
        namespaceTransformation = ArgumentGuard.IsNotNull(namespaceTransformation, nameof(namespaceTransformation));
        noNamespaceTransformation = ArgumentGuard.IsNotNull(noNamespaceTransformation, nameof(noNamespaceTransformation));
        evaluator = ArgumentGuard.IsNotNull(evaluator, nameof(evaluator));

        var results = new List<Result<T>>();

        foreach (var @interface in SourceModel.Interfaces
            .Where(x => (Settings.CopyInterfacePredicate?.Invoke(x) ?? true)
                        && (includeNonAbstractionNamespaces || Settings.BuilderAbstractionsTypeConversionNamespaces.Contains(x.GetNamespaceWithDefault()))))
        {
            var metadata = GetMappingMetadata(@interface).ToArray();
            var ns = metadata.GetStringValue(MetadataNames.CustomBuilderInterfaceNamespace);

            if (!string.IsNullOrEmpty(ns))
            {
                var property = new PropertyBuilder()
                    .WithName("Dummy")
                    .WithTypeName(@interface)
                    .Build();

                var newTypeName = metadata.GetStringValue(MetadataNames.CustomBuilderInterfaceName, "I{NoGenerics(ClassName(property.TypeName))}Builder{GenericArguments(property.TypeName, true)}");
                var newFullName = $"{ns}.{newTypeName}";

                var result = (await evaluator.EvaluateInterpolatedStringAsync
                (
                    newFullName,
                    FormatProvider,
                    new ParentChildContext<BuilderContext, Property>(this, property, Settings),
                    token
                ).ConfigureAwait(false)).Transform(y => namespaceTransformation(@interface, y));

                results.Add(result);

                if (!result.IsSuccessful())
                {
                    break;
                }
            }
            else
            {
                results.Add(Result.Success(noNamespaceTransformation(@interface)));
            }
        }

        if (Settings.UseCrossCuttingInterfaces && typeof(T) == typeof(string))
        {
            results.Add(Result.FromExistingResult<T>(Result.Success(GetBuilderInterface())));
        }

        return results.ToArray();
    }

    public IEnumerable<MethodBuilder> GetFluentMethodsForCollectionProperty(Property property, IReadOnlyDictionary<string, Result<GenericFormattableString>> results, string returnType, string typeNameKey, string enumerableOverloadKeyPrefix, string arrayOverloadKeyPrefix)
    {
        yield return new MethodBuilder()
            .WithName(results.GetValue(ResultNames.AddMethodName))
            .WithReturnTypeName(returnType)
            .AddParameters(CreateParameterForBuilder(property, results.GetValue(typeNameKey).ToString().FixCollectionTypeName(typeof(IEnumerable<>).WithoutGenerics())))
            .AddCodeStatements(results.Where(x => x.Key.StartsWith(enumerableOverloadKeyPrefix)).Select(x => x.Value.Value!.ToString()));

        yield return new MethodBuilder()
            .WithName(results.GetValue(ResultNames.AddMethodName))
            .WithReturnTypeName(returnType)
            .AddParameters(CreateParameterForBuilder(property, results.GetValue(typeNameKey).ToString().FixTypeName().ConvertTypeNameToArray()).WithIsParamArray())
            .AddCodeStatements(results.Where(x => x.Key.StartsWith(arrayOverloadKeyPrefix)).Select(x => x.Value.Value!.ToString()));
    }

    public IEnumerable<MethodBuilder> GetFluentMethodsForNonCollectionProperty(Property property, IReadOnlyDictionary<string, Result<GenericFormattableString>> results, string returnType, string typeNameKey, string expressionKey)
    {
        yield return new MethodBuilder()
            .WithName(results.GetValue("MethodName"))
            .WithReturnTypeName(returnType)
            .AddParameters(CreateParameterForBuilder(property, results.GetValue(typeNameKey)))
            .Chain(method => AddNullChecks(method, results))
            .AddCodeStatements
            (
                results.GetValue(expressionKey),
                ReturnValueStatementForFluentMethod
            );
    }

    public ClassBuilder Builder { get; } = new();

    private string ReturnValue
    {
        get
        {
            if (IsBuilderForAbstractEntity)
            {
                return "(TBuilder)this";
            }

            return "this";
        }
    }

    private bool NeedsPragmasForBuildMethod()
        => Settings.EnableNullableReferenceTypes
        && !IsBuilderForAbstractEntity
        && !Settings.AddNullChecks;

    private string GetBuilderInterface()
    {
        if (IsBuilderForAbstractEntity)
        {
            var baseClass = Settings.BaseClass ?? SourceModel;
            return typeof(IBuilder<object>).ReplaceGenericTypeName($"{MapTypeName(baseClass.GetFullName())}{baseClass.GetGenericTypeArgumentsString()}");
        }
        else
        {
            return typeof(IBuilder<object>).ReplaceGenericTypeName(BuildReturnTypeName);
        }
    }

    public override object GetResponseBuilder() => Builder;

    public override bool SourceModelHasNoProperties() => SourceModel.Properties.Count == 0;
}

namespace ClassFramework.Pipelines.BuilderExtension.Commands;

public class GenerateBuilderExtensionCommand(TypeBase sourceModel, PipelineSettings settings, IFormatProvider formatProvider) : CommandBase<TypeBase>(sourceModel, settings, formatProvider)
{
    private const string Instance = "instance";

    protected override string NewCollectionTypeName => Settings.BuilderNewCollectionTypeName;

    public IEnumerable<Property> GetSourceProperties()
        => SourceModel.Properties.Where(x => SourceModel.IsMemberValidForBuilderClass(x, Settings));

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

    public override bool SourceModelHasNoProperties() => SourceModel.Properties.Count == 0;

    public override async Task<Result<TypeBaseBuilder>> ExecuteCommandAsync<TContext>(ICommandService commandService, TContext command, CancellationToken token)
    {
        commandService = ArgumentGuard.IsNotNull(commandService, nameof(commandService));
        command = ArgumentGuard.IsNotNull(command, nameof(command));

        return (await commandService.ExecuteAsync<TContext, ClassBuilder>(command, token).ConfigureAwait(false))
            .TryCast<TypeBaseBuilder>();
    }
}

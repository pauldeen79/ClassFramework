namespace ClassFramework.Pipelines.Entity.Components;

public class AddInterfacesComponent(IExpressionEvaluator evaluator) : IPipelineComponent<EntityContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ProcessAsync(PipelineContext<EntityContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Request.Settings.UseCrossCuttingInterfaces)
        {
            var results = await new AsyncResultDictionaryBuilder<GenericFormattableString>()
                .Add(NamedResults.Name, _evaluator.EvaluateInterpolatedStringAsync(context.Request.Settings.EntityNameFormatString, context.Request.FormatProvider, context.Request, token))
                .Add(NamedResults.Namespace, context.Request.GetMappingMetadata(context.Request.SourceModel.GetFullName()).GetGenericFormattableStringAsync(MetadataNames.CustomEntityNamespace, _evaluator.EvaluateInterpolatedStringAsync(context.Request.Settings.EntityNamespaceFormatString, context.Request.FormatProvider, context.Request, token)))
                .Add("BuilderInterfaceNamespace", context.Request.GetMappingMetadata(context.Request.SourceModel.GetFullName()).GetGenericFormattableStringAsync(MetadataNames.CustomBuilderInterfaceNamespace, _evaluator.EvaluateInterpolatedStringAsync(context.Request.Settings.BuilderNamespaceFormatString, context.Request.FormatProvider, context.Request, token)))
                .Add("ToBuilderMethodName", _evaluator.EvaluateInterpolatedStringAsync(context.Request.Settings.ToBuilderFormatString, context.Request.FormatProvider, context.Request, token))
                .Add("ToTypedBuilderMethodName", _evaluator.EvaluateInterpolatedStringAsync(context.Request.Settings.ToTypedBuilderFormatString, context.Request.FormatProvider, context.Request, token))
                .Add("BuilderName", _evaluator.EvaluateInterpolatedStringAsync(context.Request.Settings.BuilderNameFormatString, context.Request.FormatProvider, context.Request, token))
                .Build()
                .ConfigureAwait(false);

            var error = results.GetError();
            if (error is not null)
            {
                // Error in formattable string parsing
                return error;
            }

            var methodName = results.GetValue("ToBuilderMethodName");
            if (string.IsNullOrEmpty(methodName))
            {
                return Result.Success();
            }

            var typedMethodName = results.GetValue("ToTypedBuilderMethodName");

            var ns = results.GetValue(NamedResults.Namespace).ToString();
            var name = results.GetValue(NamedResults.Name).ToString();

            var entityFullName = $"{ns.AppendWhenNotNullOrEmpty(".")}{name}";
            if (context.Request.Settings.EnableInheritance && context.Request.Settings.BaseClass is not null)
            {
                entityFullName = entityFullName.ReplaceSuffix("Base", string.Empty, StringComparison.Ordinal);
            }

            var entityConcreteFullName = context.Request.Settings.EnableInheritance && context.Request.Settings.BaseClass is not null
                ? context.Request.Settings.BaseClass.GetFullName()
                : entityFullName;
            
            var metadata = context.Request.GetMappingMetadata(entityFullName).ToArray();
            var customNamespaceResults = new ResultDictionaryBuilder<string>()
                .Add("CustomBuilderNamespace", () => metadata.GetStringResult(MetadataNames.CustomBuilderNamespace, () => Result.Success($"{ns.AppendWhenNotNullOrEmpty(".")}Builders")))
                .Add("CustomBuilderInterfaceNamespace", () => metadata.GetStringResult(MetadataNames.CustomBuilderInterfaceNamespace, () => Result.Success(GetBuilderInterfaceNamespace(context, results, ns))))
                .Add("CustomConcreteBuilderNamespace", () => context.Request.GetMappingMetadata(entityConcreteFullName).GetStringResult(MetadataNames.CustomBuilderNamespace, () => Result.Success($"{ns.AppendWhenNotNullOrEmpty(".")}Builders")))
                .Build();

            var customNamespaceError = customNamespaceResults.GetError();
            if (customNamespaceError is not null)
            {
                // Error in formattable string parsing
                return customNamespaceError;
            }

            var builderConcreteName = context.Request.Settings.EnableInheritance && context.Request.Settings.BaseClass is null
                ? name
                : name.ReplaceSuffix("Base", string.Empty, StringComparison.Ordinal);

            var generics = context.Request.SourceModel.GetGenericTypeArgumentsString();
            var builderName = results.GetValue("BuilderName").ToString().Replace(context.Request.SourceModel.Name, builderConcreteName);
            var builderConcreteTypeName = $"{customNamespaceResults.GetValue("CustomBuilderNamespace")}.{builderName}";
            var builderTypeName = context.Request.GetBuilderTypeName(customNamespaceResults.GetValue("CustomBuilderInterfaceNamespace"), customNamespaceResults.GetValue("CustomConcreteBuilderNamespace"), builderConcreteName, builderConcreteTypeName, results.GetValue("BuilderName"));

            context.Request.Builder.AddInterfaces(typeof(IBuildableEntity<object>).ReplaceGenericTypeName(builderTypeName));
        }

        if (!context.Request.Settings.CopyInterfaces)
        {
            return Result.Success();
        }

        var baseClass = await context.Request.SourceModel.GetEntityBaseClassAsync(context.Request.Settings.EnableInheritance, context.Request.Settings.BaseClass).ConfigureAwait(false);

        context.Request.Builder.AddInterfaces(context.Request.SourceModel.Interfaces
            .Where(x => context.Request.Settings.CopyInterfacePredicate?.Invoke(x) ?? true)
            .Where(x => x != baseClass)
            .Select(x => context.Request.MapTypeName(x.FixTypeName()))
            .Where(x => !string.IsNullOrEmpty(x)));

        return Result.Success();
    }

    private static string GetBuilderInterfaceNamespace(PipelineContext<EntityContext> context, IReadOnlyDictionary<string, Result<GenericFormattableString>> results, string ns)
    => context.Request.Settings.InheritFromInterfaces
        ? results.GetValue("BuilderInterfaceNamespace").ToString()
        : $"{ns.AppendWhenNotNullOrEmpty(".")}Builders";
}

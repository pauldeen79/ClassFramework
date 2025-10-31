namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class AddExtensionMethodsForNonCollectionPropertiesComponent(IExpressionEvaluator evaluator) : IPipelineComponent<BuilderExtensionContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ExecuteAsync(BuilderExtensionContext context, ICommandService commandService, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        return await context.ProcessPropertiesAsync(
            context.Settings.SetMethodNameFormatString,
            context.GetSourceProperties().Where(x => !x.TypeName.FixTypeName().IsCollectionTypeName()),
            GetResultsAsync,
            context.GetReturnTypeForFluentMethod,
            (property, returnType, results, token) => AddMethods(context, property, returnType, results),
            token).ConfigureAwait(false);
    }

    private static void AddMethods(BuilderExtensionContext context, Property property, string returnType, IReadOnlyDictionary<string, Result<GenericFormattableString>> results)
    {
        context.Builder.AddMethods(context.GetFluentMethodsForNonCollectionProperty(property, results, returnType, ResultNames.TypeName, ResultNames.BuilderWithExpression));

        if (results.NeedNonLazyOverloads())
        {
            //Add overload for non-func type
            context.Builder.AddMethods(context.GetFluentMethodsForNonCollectionProperty(property, results, returnType, ResultNames.NonLazyTypeName, ResultNames.BuilderNonLazyWithExpression));
        }
    }

    private Task<IReadOnlyDictionary<string, Result<GenericFormattableString>>> GetResultsAsync(BuilderExtensionContext context, Property property, CancellationToken token)
        => context.GetResultsForBuilderNonCollectionProperties(
            property,
            new ParentChildContext<BuilderExtensionContext, Property>(context, property, context.Settings),
            _evaluator);
}

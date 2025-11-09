namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class AddExtensionMethodsForNonCollectionPropertiesComponent(IExpressionEvaluator evaluator) : IPipelineComponent<GenerateBuilderExtensionCommand, ClassBuilder>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ExecuteAsync(GenerateBuilderExtensionCommand context, ClassBuilder response, ICommandService commandService, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));
        response = response.IsNotNull(nameof(response));

        return await context.ProcessPropertiesAsync(
            context.Settings.SetMethodNameFormatString,
            context.GetSourceProperties().Where(x => !x.TypeName.FixTypeName().IsCollectionTypeName()),
            GetResultsAsync,
            context.GetReturnTypeForFluentMethod,
            (property, returnType, results, token) => AddMethods(context, response, property, returnType, results),
            token).ConfigureAwait(false);
    }

    private static void AddMethods(GenerateBuilderExtensionCommand context, ClassBuilder response, Property property, string returnType, IReadOnlyDictionary<string, Result<GenericFormattableString>> results)
    {
        response.AddMethods(context.GetFluentMethodsForNonCollectionProperty(property, results, returnType, ResultNames.TypeName, ResultNames.BuilderWithExpression));

        if (results.NeedNonLazyOverloads())
        {
            //Add overload for non-func type
            response.AddMethods(context.GetFluentMethodsForNonCollectionProperty(property, results, returnType, ResultNames.NonLazyTypeName, ResultNames.BuilderNonLazyWithExpression));
        }
    }

    private Task<IReadOnlyDictionary<string, Result<GenericFormattableString>>> GetResultsAsync(GenerateBuilderExtensionCommand context, Property property, CancellationToken token)
        => context.GetResultsForBuilderNonCollectionProperties(
            property,
            new ParentChildContext<GenerateBuilderExtensionCommand, Property>(context, property, context.Settings),
            _evaluator);
}

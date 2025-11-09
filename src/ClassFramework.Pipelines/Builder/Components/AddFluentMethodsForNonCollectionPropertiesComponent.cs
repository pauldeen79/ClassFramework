namespace ClassFramework.Pipelines.Builder.Components;

public class AddFluentMethodsForNonCollectionPropertiesComponent(IExpressionEvaluator evaluator) : IPipelineComponent<GenerateBuilderCommand, ClassBuilder>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ExecuteAsync(GenerateBuilderCommand command, ClassBuilder response, ICommandService commandService, CancellationToken token)
    {
        command = command.IsNotNull(nameof(command));
        response = response.IsNotNull(nameof(response));

        return await command.ProcessPropertiesAsync(
            command.Settings.SetMethodNameFormatString,
            command.GetSourceProperties().Where(x => command.IsValidForFluentMethod(x) && !x.TypeName.FixTypeName().IsCollectionTypeName()),
            GetResultsAsync,
            command.GetReturnTypeForFluentMethod,
            (property, returnType, results, token) => AddMethods(command, response, property, returnType, results),
            token).ConfigureAwait(false);
    }

    private static void AddMethods(GenerateBuilderCommand command, ClassBuilder response, Property property, string returnType, IReadOnlyDictionary<string, Result<GenericFormattableString>> results)
    {
        response.AddMethods(command.GetFluentMethodsForNonCollectionProperty(property, results, returnType, ResultNames.TypeName, ResultNames.BuilderWithExpression));

        if (results.NeedNonLazyOverloads())
        {
            //Add overload for non-func type
            response.AddMethods(command.GetFluentMethodsForNonCollectionProperty(property, results, returnType, ResultNames.NonLazyTypeName, ResultNames.BuilderNonLazyWithExpression));
        }
    }

    private Task<IReadOnlyDictionary<string, Result<GenericFormattableString>>> GetResultsAsync(GenerateBuilderCommand command, Property property, CancellationToken token)
        => command.GetResultsForBuilderNonCollectionPropertiesAsync(
            property,
            new ParentChildContext<GenerateBuilderCommand, Property>(command, property, command.Settings),
            _evaluator,
            token);
}

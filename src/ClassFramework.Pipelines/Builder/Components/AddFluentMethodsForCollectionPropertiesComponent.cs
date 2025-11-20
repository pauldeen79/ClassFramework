namespace ClassFramework.Pipelines.Builder.Components;

public class AddFluentMethodsForCollectionPropertiesComponent(IExpressionEvaluator evaluator) : IPipelineComponent<GenerateBuilderCommand, ClassBuilder>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ExecuteAsync(GenerateBuilderCommand command, ClassBuilder response, ICommandService commandService, CancellationToken token)
    {
        command = command.IsNotNull(nameof(command));
        response = response.IsNotNull(nameof(response));

        return await command.ProcessPropertiesAsync(
            command.Settings.AddMethodNameFormatString,
            command.GetSourceProperties().Where(x => command.IsValidForFluentMethod(x) && x.TypeName.FixTypeName().IsCollectionTypeName()),
            GetResultsAsync,
            command.GetReturnTypeForFluentMethod,
            (property, returnType, results, token) => AddMethods(command, response, property, returnType, results),
            token).ConfigureAwait(false);
    }

    private static void AddMethods(GenerateBuilderCommand command, ClassBuilder response, Property property, string returnType, IReadOnlyDictionary<string, Result<GenericFormattableString>> results)
    {
        response.AddMethods(command.GetFluentMethodsForCollectionProperty(property, results, returnType, ResultNames.TypeName, "EnumerableOverload.", "ArrayOverload."));

        if (results.NeedNonLazyOverloads())
        {
            //Add overloads for non-func type
            response.AddMethods(command.GetFluentMethodsForCollectionProperty(property, results, returnType, ResultNames.NonLazyTypeName, "NonLazyEnumerableOverload.", "NonLazyArrayOverload."));
        }
    }

    private async Task<IReadOnlyDictionary<string, Result<GenericFormattableString>>> GetResultsAsync(GenerateBuilderCommand command, Property property, CancellationToken token)
    {
        var parentChildContext = new ParentChildContext<GenerateBuilderCommand, Property>(command, property, command.Settings);

        return await command.GetResultDictionaryForBuilderCollectionProperties(property, parentChildContext, _evaluator, token)
            .AddRange("EnumerableOverload.{0}", await GetCodeStatementsForEnumerableOverloadAsync(command, property, parentChildContext, false, token).ConfigureAwait(false))
            .AddRange("ArrayOverload.{0}", await GetCodeStatementsForArrayOverloadAsync(command, property, false, token).ConfigureAwait(false))
            .AddRange("NonLazyArrayOverload.{0}", await GetCodeStatementsForArrayOverloadAsync(command, property, true, token).ConfigureAwait(false))
            .AddRange("NonLazyEnumerableOverload.{0}", await GetCodeStatementsForEnumerableOverloadAsync(command, property, parentChildContext, true, token).ConfigureAwait(false))
            .BuildAsync(token)
            .ConfigureAwait(false);
    }

    private async Task<IEnumerable<Result<GenericFormattableString>>> GetCodeStatementsForEnumerableOverloadAsync(GenerateBuilderCommand command, Property property, ParentChildContext<GenerateBuilderCommand, Property> parentChildContext, bool useBuilderLazyValues, CancellationToken token)
    {
        if (command.Settings.BuilderNewCollectionTypeName == typeof(IEnumerable<>).WithoutGenerics())
        {
            // When using IEnumerable<>, do not call ToArray because we want lazy evaluation
            return await GetCodeStatementsForArrayOverloadAsync(command, property, useBuilderLazyValues, token).ConfigureAwait(false);
        }

        var results = new List<Result<GenericFormattableString>>();

        // When not using IEnumerable<>, we can simply force ToArray because it's stored in a generic list or collection of some sort anyway.
        // (in other words, materialization is always performed)
        if (command.Settings.AddNullChecks)
        {
            results.Add(Result.Success<GenericFormattableString>(command.CreateArgumentNullException(property.Name.ToCamelCase(command.FormatProvider.ToCultureInfo()).GetCsharpFriendlyName())));
        }

        results.Add(await _evaluator.EvaluateInterpolatedStringAsync("return {addMethodNameFormatString}({CsharpFriendlyName(property.Name.ToCamelCase())}.ToArray());", command.FormatProvider, parentChildContext, token).ConfigureAwait(false));

        return results;
    }

    private async Task<IEnumerable<Result<GenericFormattableString>>> GetCodeStatementsForArrayOverloadAsync(GenerateBuilderCommand command, Property property, bool useBuilderLazyValues, CancellationToken token)
    {
        var results = new List<Result<GenericFormattableString>>();

        if (command.Settings.AddNullChecks)
        {
            var argumentNullCheckResult = await _evaluator.EvaluateInterpolatedStringAsync
            (
                command.GetMappingMetadata(property.TypeName).GetStringValue(MetadataNames.CustomBuilderArgumentNullCheckExpression, "{ArgumentNullCheck()}"),
                command.FormatProvider,
                new ParentChildContext<GenerateBuilderCommand, Property>(command, property, command.Settings),
                token
            ).ConfigureAwait(false);

            if (!argumentNullCheckResult.IsSuccessful() || !string.IsNullOrEmpty(argumentNullCheckResult.Value?.ToString()))
            {
                results.Add(argumentNullCheckResult);
            }
        }

        var builderAddExpressionResult = await _evaluator.EvaluateInterpolatedStringAsync
        (
            command
                .GetMappingMetadata(property.TypeName)
                .GetStringValue(MetadataNames.CustomBuilderAddExpression, useBuilderLazyValues
                    ? command.Settings.NonLazyCollectionCopyStatementFormatString
                    : command.Settings.CollectionCopyStatementFormatString),
            command.FormatProvider,
            new ParentChildContext<GenerateBuilderCommand, Property>(command, property, command.Settings),
            token
        ).ConfigureAwait(false);

        results.Add(builderAddExpressionResult);
        results.Add(Result.Success<GenericFormattableString>(command.ReturnValueStatementForFluentMethod));

        return results;
    }
}

namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class AddExtensionMethodsForCollectionPropertiesComponent(IExpressionEvaluator evaluator) : IPipelineComponent<GenerateBuilderExtensionCommand, ClassBuilder>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ExecuteAsync(GenerateBuilderExtensionCommand command, ClassBuilder response, ICommandService commandService, CancellationToken token)
    {
        command = command.IsNotNull(nameof(command));
        response = response.IsNotNull(nameof(response));

        return await command.ProcessPropertiesAsync(
            command.Settings.AddMethodNameFormatString,
            command.GetSourceProperties().Where(x => x.TypeName.FixTypeName().IsCollectionTypeName()),
            GetResultsAsync,
            command.GetReturnTypeForFluentMethod,
            (property, returnType, results, token) => AddMethods(command, response, property, returnType, results),
            token).ConfigureAwait(false);
    }

    private static void AddMethods(GenerateBuilderExtensionCommand command, ClassBuilder response, Property property, string returnType, IReadOnlyDictionary<string, Result<GenericFormattableString>> results)
    {
        response.AddMethods(command.GetFluentMethodsForCollectionProperty(property, results, returnType, ResultNames.TypeName, "EnumerableOverload.", "ArrayOverload."));

        if (results.NeedNonLazyOverloads())
        {
            //Add overloads for non-func type
            response.AddMethods(command.GetFluentMethodsForCollectionProperty(property, results, returnType, ResultNames.NonLazyTypeName, "NonLazyEnumerableOverload.", "NonLazyArrayOverload."));
        }
    }

    private async Task<IReadOnlyDictionary<string, Result<GenericFormattableString>>> GetResultsAsync(GenerateBuilderExtensionCommand command, Property property, CancellationToken token)
    {
        var parentChildContext = new ParentChildContext<GenerateBuilderExtensionCommand, Property>(command, property, command.Settings);

        return await command.GetResultDictionaryForBuilderCollectionProperties(property, parentChildContext, _evaluator, token)
            .AddRange("EnumerableOverload.{0}", await GetCodeStatementsForEnumerableOverloadAsync(command, property, parentChildContext, token).ConfigureAwait(false))
            .AddRange("ArrayOverload.{0}", await GetCodeStatementsForArrayOverloadAsync(command, property, false, token).ConfigureAwait(false))
            .AddRange("NonLazyArrayOverload.{0}", await GetCodeStatementsForArrayOverloadAsync(command, property, true, token).ConfigureAwait(false))
            .Build()
            .ConfigureAwait(false);
    }

    private async Task<IEnumerable<Result<GenericFormattableString>>> GetCodeStatementsForEnumerableOverloadAsync(GenerateBuilderExtensionCommand command, Property property, ParentChildContext<GenerateBuilderExtensionCommand, Property> parentChildContext, CancellationToken token)
    {
        var results = new List<Result<GenericFormattableString>>();

        if (command.Settings.AddNullChecks)
        {
            results.Add(Result.Success<GenericFormattableString>(command.CreateArgumentNullException(property.Name.ToCamelCase(command.FormatProvider.ToCultureInfo()).GetCsharpFriendlyName())));
        }

        results.Add(await _evaluator.EvaluateInterpolatedStringAsync("return instance.{addMethodNameFormatString}<T>({CsharpFriendlyName(property.Name.ToCamelCase())}.ToArray());", command.FormatProvider, parentChildContext, token).ConfigureAwait(false));

        return results;
    }

    private async Task<IEnumerable<Result<GenericFormattableString>>> GetCodeStatementsForArrayOverloadAsync(GenerateBuilderExtensionCommand command, Property property, bool useBuilderLazyValues, CancellationToken token)
    {
        var results = new List<Result<GenericFormattableString>>();

        if (command.Settings.AddNullChecks)
        {
            var argumentNullCheckResult = await _evaluator.EvaluateInterpolatedStringAsync
            (
                command.GetMappingMetadata(property.TypeName).GetStringValue(MetadataNames.CustomBuilderArgumentNullCheckExpression, "{ArgumentNullCheck()}"),
                command.FormatProvider,
                new ParentChildContext<GenerateBuilderExtensionCommand, Property>(command, property, command.Settings),
                token
            ).ConfigureAwait(false);

            results.Add(argumentNullCheckResult);
        }

        var builderAddExpressionResult = await _evaluator.EvaluateInterpolatedStringAsync
        (
            command
                .GetMappingMetadata(property.TypeName)
                .GetStringValue(MetadataNames.CustomBuilderAddExpression, useBuilderLazyValues
                    ? command.Settings.NonLazyBuilderExtensionsCollectionCopyStatementFormatString
                    : command.Settings.BuilderExtensionsCollectionCopyStatementFormatString),
            command.FormatProvider,
            new ParentChildContext<GenerateBuilderExtensionCommand, Property>(command, property, command.Settings),
            token
        ).ConfigureAwait(false);

        results.Add(builderAddExpressionResult);
        results.Add(Result.Success<GenericFormattableString>("return instance;"));

        return results;
    }
}

﻿namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class AddExtensionMethodsForCollectionPropertiesComponent(IExpressionEvaluator evaluator) : IPipelineComponent<BuilderExtensionContext>, IOrderContainer
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public int Order => PipelineStage.Process;

    public async Task<Result> ExecuteAsync(BuilderExtensionContext context, ICommandService commandService, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        return await context.ProcessPropertiesAsync(
            context.Settings.AddMethodNameFormatString,
            context.GetSourceProperties().Where(x => x.TypeName.FixTypeName().IsCollectionTypeName()),
            GetResultsAsync,
            context.GetReturnTypeForFluentMethod,
            (property, returnType, results, token) => AddMethods(context, property, returnType, results),
            token).ConfigureAwait(false);
    }

    private static void AddMethods(BuilderExtensionContext context, Property property, string returnType, IReadOnlyDictionary<string, Result<GenericFormattableString>> results)
    {
        context.Builder.AddMethods(context.GetFluentMethodsForCollectionProperty(property, results, returnType, ResultNames.TypeName, "EnumerableOverload.", "ArrayOverload."));

        if (results.NeedNonLazyOverloads())
        {
            //Add overloads for non-func type
            context.Builder.AddMethods(context.GetFluentMethodsForCollectionProperty(property, results, returnType, ResultNames.NonLazyTypeName, "NonLazyEnumerableOverload.", "NonLazyArrayOverload."));
        }
    }

    private async Task<IReadOnlyDictionary<string, Result<GenericFormattableString>>> GetResultsAsync(BuilderExtensionContext context, Property property, CancellationToken token)
    {
        var parentChildContext = new ParentChildContext<BuilderExtensionContext, Property>(context, property, context.Settings);

        return await context.GetResultDictionaryForBuilderCollectionProperties(property, parentChildContext, _evaluator)
            .AddRange("EnumerableOverload.{0}", await GetCodeStatementsForEnumerableOverload(context, property, parentChildContext, token).ConfigureAwait(false))
            .AddRange("ArrayOverload.{0}", await GetCodeStatementsForArrayOverload(context, property, false, token).ConfigureAwait(false))
            .AddRange("NonLazyArrayOverload.{0}", await GetCodeStatementsForArrayOverload(context, property, true, token).ConfigureAwait(false))
            .Build()
            .ConfigureAwait(false);
    }

    private async Task<IEnumerable<Result<GenericFormattableString>>> GetCodeStatementsForEnumerableOverload(BuilderExtensionContext context, Property property, ParentChildContext<BuilderExtensionContext, Property> parentChildContext, CancellationToken token)
    {
        var results = new List<Result<GenericFormattableString>>();

        if (context.Settings.AddNullChecks)
        {
            results.Add(Result.Success<GenericFormattableString>(context.CreateArgumentNullException(property.Name.ToCamelCase(context.FormatProvider.ToCultureInfo()).GetCsharpFriendlyName())));
        }

        results.Add(await _evaluator.EvaluateInterpolatedStringAsync("return instance.{addMethodNameFormatString}<T>({CsharpFriendlyName(property.Name.ToCamelCase())}.ToArray());", context.FormatProvider, parentChildContext, token).ConfigureAwait(false));

        return results;
    }

    private async Task<IEnumerable<Result<GenericFormattableString>>> GetCodeStatementsForArrayOverload(BuilderExtensionContext context, Property property, bool useBuilderLazyValues, CancellationToken token)
    {
        var results = new List<Result<GenericFormattableString>>();

        if (context.Settings.AddNullChecks)
        {
            var argumentNullCheckResult = await _evaluator.EvaluateInterpolatedStringAsync
            (
                context.GetMappingMetadata(property.TypeName).GetStringValue(MetadataNames.CustomBuilderArgumentNullCheckExpression, "{ArgumentNullCheck()}"),
                context.FormatProvider,
                new ParentChildContext<BuilderExtensionContext, Property>(context, property, context.Settings),
                token
            ).ConfigureAwait(false);

            results.Add(argumentNullCheckResult);
        }

        var builderAddExpressionResult = await _evaluator.EvaluateInterpolatedStringAsync
        (
            context
                .GetMappingMetadata(property.TypeName)
                .GetStringValue(MetadataNames.CustomBuilderAddExpression, useBuilderLazyValues
                    ? context.Settings.NonLazyBuilderExtensionsCollectionCopyStatementFormatString
                    : context.Settings.BuilderExtensionsCollectionCopyStatementFormatString),
            context.FormatProvider,
            new ParentChildContext<BuilderExtensionContext, Property>(context, property, context.Settings),
            token
        ).ConfigureAwait(false);

        results.Add(builderAddExpressionResult);
        results.Add(Result.Success<GenericFormattableString>("return instance;"));

        return results;
    }
}

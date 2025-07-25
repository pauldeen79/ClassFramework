namespace ClassFramework.Pipelines.Builder.Components;

public class AddFluentMethodsForCollectionPropertiesComponent(IExpressionEvaluator evaluator) : IPipelineComponent<BuilderContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ProcessAsync(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (string.IsNullOrEmpty(context.Request.Settings.AddMethodNameFormatString))
        {
            return Result.Success();
        }

        foreach (var property in context.Request.GetSourceProperties()
            .Where(x => context.Request.IsValidForFluentMethod(x) && x.TypeName.FixTypeName().IsCollectionTypeName()))
        {
            var parentChildContext = new ParentChildContext<PipelineContext<BuilderContext>, Property>(context, property, context.Request.Settings);

            var results = await context.Request.GetResultDictionaryForBuilderCollectionProperties(property, parentChildContext, _evaluator)
                .AddRange("EnumerableOverload.{0}", await GetCodeStatementsForEnumerableOverload(context, property, parentChildContext, false, token).ConfigureAwait(false))
                .AddRange("ArrayOverload.{0}", await GetCodeStatementsForArrayOverload(context, property, false, token).ConfigureAwait(false))
                .AddRange("NonLazyArrayOverload.{0}", await GetCodeStatementsForArrayOverload(context, property, true, token).ConfigureAwait(false))
                .AddRange("NonLazyEnumerableOverload.{0}", await GetCodeStatementsForEnumerableOverload(context, property, parentChildContext, true, token).ConfigureAwait(false))
                .Build()
                .ConfigureAwait(false);

            var error = results.GetError();
            if (error is not null)
            {
                // Error in formattable string parsing
                return error;
            }

            var returnType = context.Request.GetReturnTypeForFluentMethod(results.GetValue(ResultNames.Namespace), results.GetValue(ResultNames.BuilderName));

            context.Request.Builder.AddMethods(context.Request.GetFluentMethodsForCollectionProperty(property, results, returnType, ResultNames.TypeName, "EnumerableOverload.", "ArrayOverload."));

            if (results.NeedNonLazyOverloads())
            {
                //Add overloads for non-func type
                context.Request.Builder.AddMethods(context.Request.GetFluentMethodsForCollectionProperty(property, results, returnType, ResultNames.NonLazyTypeName, "NonLazyEnumerableOverload.", "NonLazyArrayOverload."));
            }
        }

        return Result.Success();
    }

    private async Task<IEnumerable<Result<GenericFormattableString>>> GetCodeStatementsForEnumerableOverload(PipelineContext<BuilderContext> context, Property property, ParentChildContext<PipelineContext<BuilderContext>, Property> parentChildContext, bool useBuilderLazyValues, CancellationToken token)
    {
        if (context.Request.Settings.BuilderNewCollectionTypeName == typeof(IEnumerable<>).WithoutGenerics())
        {
            // When using IEnumerable<>, do not call ToArray because we want lazy evaluation
            return await GetCodeStatementsForArrayOverload(context, property, useBuilderLazyValues, token).ConfigureAwait(false);
        }

        var results = new List<Result<GenericFormattableString>>();

        // When not using IEnumerable<>, we can simply force ToArray because it's stored in a generic list or collection of some sort anyway.
        // (in other words, materialization is always performed)
        if (context.Request.Settings.AddNullChecks)
        {
            results.Add(Result.Success<GenericFormattableString>(context.Request.CreateArgumentNullException(property.Name.ToCamelCase(context.Request.FormatProvider.ToCultureInfo()).GetCsharpFriendlyName())));
        }

        results.Add(await _evaluator.EvaluateInterpolatedStringAsync("return {addMethodNameFormatString}({CsharpFriendlyName(property.Name.ToCamelCase())}.ToArray());", context.Request.FormatProvider, parentChildContext, token).ConfigureAwait(false));

        return results;
    }

    private async Task<IEnumerable<Result<GenericFormattableString>>> GetCodeStatementsForArrayOverload(PipelineContext<BuilderContext> context, Property property, bool useBuilderLazyValues, CancellationToken token)
    {
        var results = new List<Result<GenericFormattableString>>();

        if (context.Request.Settings.AddNullChecks)
        {
            var argumentNullCheckResult = await _evaluator.EvaluateInterpolatedStringAsync
            (
                context.Request.GetMappingMetadata(property.TypeName).GetStringValue(MetadataNames.CustomBuilderArgumentNullCheckExpression, "{ArgumentNullCheck()}"),
                context.Request.FormatProvider,
                new ParentChildContext<PipelineContext<BuilderContext>, Property>(context, property, context.Request.Settings),
                token
            ).ConfigureAwait(false);

            results.Add(argumentNullCheckResult);
        }

        var builderAddExpressionResult = await _evaluator.EvaluateInterpolatedStringAsync
        (
            context.Request
                .GetMappingMetadata(property.TypeName)
                .GetStringValue(MetadataNames.CustomBuilderAddExpression, useBuilderLazyValues
                    ? context.Request.Settings.NonLazyCollectionCopyStatementFormatString
                    : context.Request.Settings.CollectionCopyStatementFormatString),
            context.Request.FormatProvider,
            new ParentChildContext<PipelineContext<BuilderContext>, Property>(context, property, context.Request.Settings),
            token
        ).ConfigureAwait(false);

        results.Add(builderAddExpressionResult);
        results.Add(Result.Success<GenericFormattableString>(context.Request.ReturnValueStatementForFluentMethod));

        return results;
    }
}

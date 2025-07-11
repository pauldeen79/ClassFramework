﻿namespace ClassFramework.Pipelines.Builder.Components;

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
            var parentChildContext = CreateParentChildContext(context, property);

            var results = await context.Request.GetResultsForBuilderCollectionProperties
            (
                property,
                parentChildContext,
                _evaluator,
                await GetCodeStatementsForEnumerableOverload(context, property, parentChildContext, token).ConfigureAwait(false),
                await GetCodeStatementsForArrayOverload(context, property, token).ConfigureAwait(false)
            ).ConfigureAwait(false);

            var error = results.GetError();
            if (error is not null)
            {
                // Error in formattable string parsing
                return error;
            }

            var returnType = context.Request.IsBuilderForAbstractEntity
                ? $"TBuilder{context.Request.SourceModel.GetGenericTypeArgumentsString()}"
                : $"{results.GetValue(ResultNames.Namespace).ToString().AppendWhenNotNullOrEmpty(".")}{results.GetValue(NamedResults.BuilderName)}{context.Request.SourceModel.GetGenericTypeArgumentsString()}";

            context.Request.Builder.AddMethods(new MethodBuilder()
                .WithName(results.GetValue("AddMethodName"))
                .WithReturnTypeName(returnType)
                .AddParameters(context.Request.CreateParameterForBuilder(property, results.GetValue(ResultNames.TypeName).ToString().FixCollectionTypeName(typeof(IEnumerable<>).WithoutGenerics())))
                .AddStringCodeStatements(results.Where(x => x.Key.StartsWith("EnumerableOverload.")).Select(x => x.Value.Value!.ToString()))
            );

            context.Request.Builder.AddMethods(new MethodBuilder()
                .WithName(results.GetValue("AddMethodName"))
                .WithReturnTypeName(returnType)
                .AddParameters(context.Request.CreateParameterForBuilder(property, results.GetValue(ResultNames.TypeName).ToString().FixTypeName().ConvertTypeNameToArray()).WithIsParamArray())
                .AddStringCodeStatements(results.Where(x => x.Key.StartsWith("ArrayOverload.")).Select(x => x.Value.Value!.ToString()))
            );
        }

        return Result.Success();
    }

    private async Task<IEnumerable<Result<GenericFormattableString>>> GetCodeStatementsForEnumerableOverload(PipelineContext<BuilderContext> context, Property property, ParentChildContext<PipelineContext<BuilderContext>, Property> parentChildContext, CancellationToken token)
    {
        if (context.Request.Settings.BuilderNewCollectionTypeName == typeof(IEnumerable<>).WithoutGenerics())
        {
            // When using IEnumerable<>, do not call ToArray because we want lazy evaluation
            return await GetCodeStatementsForArrayOverload(context, property, token).ConfigureAwait(false);
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

    private async Task<IEnumerable<Result<GenericFormattableString>>> GetCodeStatementsForArrayOverload(PipelineContext<BuilderContext> context, Property property, CancellationToken token)
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

            if (!argumentNullCheckResult.IsSuccessful() || !string.IsNullOrEmpty(argumentNullCheckResult.Value!.ToString()))
            {
                results.Add(argumentNullCheckResult);
            }
        }

        var builderAddExpressionResult = await _evaluator.EvaluateInterpolatedStringAsync
        (
            context.Request.GetMappingMetadata(property.TypeName).GetStringValue(MetadataNames.CustomBuilderAddExpression, context.Request.Settings.CollectionCopyStatementFormatString),
            context.Request.FormatProvider,
            new ParentChildContext<PipelineContext<BuilderContext>, Property>(context, property, context.Request.Settings),
            token
        ).ConfigureAwait(false);


        results.Add(builderAddExpressionResult);
        results.Add(Result.Success<GenericFormattableString>(context.Request.ReturnValueStatementForFluentMethod));

        return results;
    }

    private static ParentChildContext<PipelineContext<BuilderContext>, Property> CreateParentChildContext(PipelineContext<BuilderContext> context, Property property)
        => new(context, property, context.Request.Settings);
}

namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class AddExtensionMethodsForCollectionPropertiesComponent(IExpressionEvaluator evaluator) : IPipelineComponent<BuilderExtensionContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ProcessAsync(PipelineContext<BuilderExtensionContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (string.IsNullOrEmpty(context.Request.Settings.AddMethodNameFormatString))
        {
            return Result.Success();
        }

        foreach (var property in context.Request.GetSourceProperties().Where(x => x.TypeName.FixTypeName().IsCollectionTypeName()))
        {
            var parentChildContext = CreateParentChildContext(context, property);

            var results = await context.Request.GetResultsForBuilderCollectionProperties(property, parentChildContext, _evaluator, await GetCodeStatementsForEnumerableOverload(context, property, parentChildContext, token).ConfigureAwait(false), await GetCodeStatementsForArrayOverload(context, property, token).ConfigureAwait(false), token).ConfigureAwait(false);

            var error = results.GetError();
            if (error is not null)
            {
                // Error in formattable string parsing
                return error;
            }

            var returnType = $"{results["Namespace"].Value!.ToString().AppendWhenNotNullOrEmpty(".")}{results["BuilderName"].Value}{context.Request.SourceModel.GetGenericTypeArgumentsString()}";

            context.Request.Builder.AddMethods(new MethodBuilder()
                .WithName(results["AddMethodName"].Value!)
                .WithReturnTypeName("T")
                .WithStatic()
                .WithExtensionMethod()
                .AddGenericTypeArguments("T")
                .AddGenericTypeArgumentConstraints($"where T : {returnType}")
                .AddParameter("instance", "T")
                .AddParameters(context.Request.CreateParameterForBuilder(property, results["TypeName"].Value!.ToString().FixCollectionTypeName(typeof(IEnumerable<>).WithoutGenerics())))
                .AddStringCodeStatements(results.Where(x => x.Key.StartsWith("EnumerableOverload.")).Select(x => x.Value.Value!.ToString()))
            );

            context.Request.Builder.AddMethods(new MethodBuilder()
                .WithName(results["AddMethodName"].Value!)
                .WithReturnTypeName("T")
                .WithStatic()
                .WithExtensionMethod()
                .AddGenericTypeArguments("T")
                .AddGenericTypeArgumentConstraints($"where T : {returnType}")
                .AddParameter("instance", "T")
                .AddParameters(context.Request.CreateParameterForBuilder(property, results["TypeName"].Value!.ToString().FixTypeName().ConvertTypeNameToArray()).WithIsParamArray())
                .AddStringCodeStatements(results.Where(x => x.Key.StartsWith("ArrayOverload.")).Select(x => x.Value.Value!.ToString()))
            );
        }

        return Result.Success();
    }

    private async Task<IEnumerable<Result<GenericFormattableString>>> GetCodeStatementsForEnumerableOverload(PipelineContext<BuilderExtensionContext> context, Property property, ParentChildContext<PipelineContext<BuilderExtensionContext>, Property> parentChildContext, CancellationToken token)
    {
        if (context.Request.Settings.AddNullChecks)
        {
            return [Result.Success<GenericFormattableString>(context.Request.CreateArgumentNullException(property.Name.ToCamelCase(context.Request.FormatProvider.ToCultureInfo()).GetCsharpFriendlyName()))];
        }

        return [await _evaluator.EvaluateAsync("return instance.{$addMethodNameFormatString}<T>({CsharpFriendlyName(ToCamelCase(property.Name))}.ToArray());", context.Request.FormatProvider, parentChildContext, token).ConfigureAwait(false)];
    }

    private async Task<IEnumerable<Result<GenericFormattableString>>> GetCodeStatementsForArrayOverload(PipelineContext<BuilderExtensionContext> context, Property property, CancellationToken token)
    {
        if (context.Request.Settings.AddNullChecks)
        {
            var argumentNullCheckResult = await _evaluator.EvaluateAsync
            (
                context.Request.GetMappingMetadata(property.TypeName).GetStringValue(MetadataNames.CustomBuilderArgumentNullCheckExpression, "{NullCheck.Argument}"),
                context.Request.FormatProvider,
                new ParentChildContext<PipelineContext<BuilderExtensionContext>, Property>(context, property, context.Request.Settings),
                token
            ).ConfigureAwait(false);
            return [argumentNullCheckResult];
        }

        var builderAddExpressionResult = await _evaluator.EvaluateAsync
        (
            context.Request
                .GetMappingMetadata(property.TypeName)
                .GetStringValue(MetadataNames.CustomBuilderAddExpression, context.Request.Settings.CollectionCopyStatementFormatString),
            context.Request.FormatProvider,
            new ParentChildContext<PipelineContext<BuilderExtensionContext>, Property>(context, property, context.Request.Settings),
            token
        ).ConfigureAwait(false);

        return [builderAddExpressionResult, Result.Success<GenericFormattableString>("return instance;")];
    }

    private static ParentChildContext<PipelineContext<BuilderExtensionContext>, Property> CreateParentChildContext(PipelineContext<BuilderExtensionContext> context, Property property)
        => new(context, property, context.Request.Settings);
}

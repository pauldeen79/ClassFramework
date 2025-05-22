namespace ClassFramework.Pipelines.Builder.Components;

public class AddFluentMethodsForNonCollectionPropertiesComponent(IExpressionEvaluator evaluator) : IPipelineComponent<BuilderContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public Task<Result> ProcessAsync(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (string.IsNullOrEmpty(context.Request.Settings.SetMethodNameFormatString))
        {
            return Task.FromResult(Result.Success());
        }

        foreach (var property in context.Request.GetSourceProperties().Where(x => context.Request.IsValidForFluentMethod(x) && !x.TypeName.FixTypeName().IsCollectionTypeName()))
        {
            var parentChildContext = new ParentChildContext<PipelineContext<BuilderContext>, Property>(context, property, context.Request.Settings);

            var results = context.Request.GetResultsForBuilderNonCollectionProperties(property, parentChildContext, _evaluator);

            var error = results.GetError();
            if (error is not null)
            {
                // Error in formattable string parsing
                return Task.FromResult<Result>(error);
            }

            var builder = new MethodBuilder()
                .WithName(results["MethodName"].Value!)
                .WithReturnTypeName(context.Request.IsBuilderForAbstractEntity
                      ? $"TBuilder{context.Request.SourceModel.GetGenericTypeArgumentsString()}"
                      : $"{results["Namespace"].Value!.ToString().AppendWhenNotNullOrEmpty(".")}{results["BuilderName"].Value}{context.Request.SourceModel.GetGenericTypeArgumentsString()}")
                .AddParameters(context.Request.CreateParameterForBuilder(property, results["TypeName"].Value!));

            context.Request.AddNullChecks(builder, results);

            builder.AddStringCodeStatements
            (
                results["BuilderWithExpression"].Value!,
                context.Request.ReturnValueStatementForFluentMethod
            );

            context.Request.Builder.AddMethods(builder);
        }

        return Task.FromResult(Result.Success());
    }
}

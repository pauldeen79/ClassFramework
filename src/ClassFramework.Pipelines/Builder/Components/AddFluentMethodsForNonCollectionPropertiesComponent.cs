namespace ClassFramework.Pipelines.Builder.Components;

public class AddFluentMethodsForNonCollectionPropertiesComponent(IExpressionEvaluator evaluator) : IPipelineComponent<BuilderContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ProcessAsync(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (string.IsNullOrEmpty(context.Request.Settings.SetMethodNameFormatString))
        {
            return Result.Success();
        }

        foreach (var property in context.Request.GetSourceProperties()
            .Where(x => context.Request.IsValidForFluentMethod(x) && !x.TypeName.FixTypeName().IsCollectionTypeName()))
        {
            var parentChildContext = new ParentChildContext<PipelineContext<BuilderContext>, Property>(context, property, context.Request.Settings);

            var results = await context.Request.GetResultsForBuilderNonCollectionProperties(property, parentChildContext, _evaluator).ConfigureAwait(false);

            var error = results.GetError();
            if (error is not null)
            {
                // Error in formattable string parsing
                return error;
            }

            var builder = new MethodBuilder()
                .WithName(results.GetValue("MethodName"))
                .WithReturnTypeName(context.Request.IsBuilderForAbstractEntity
                      ? $"TBuilder{context.Request.SourceModel.GetGenericTypeArgumentsString()}"
                      : $"{results.GetValue(ResultNames.Namespace).ToString().AppendWhenNotNullOrEmpty(".")}{results.GetValue(NamedResults.BuilderName)}{context.Request.SourceModel.GetGenericTypeArgumentsString()}")
                .AddParameters(context.Request.CreateParameterForBuilder(property, results.GetValue(ResultNames.TypeName)));

            context.Request.AddNullChecks(builder, results);

            builder.AddCodeStatements
            (
                results.GetValue("BuilderWithExpression"),
                context.Request.ReturnValueStatementForFluentMethod
            );

            context.Request.Builder.AddMethods(builder);
        }

        return Result.Success();
    }
}

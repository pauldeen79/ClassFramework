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

            context.Request.Builder.AddMethods(new MethodBuilder()
                .WithName(results.GetValue("MethodName"))
                .WithReturnTypeName(context.Request.IsBuilderForAbstractEntity
                      ? $"TBuilder{context.Request.SourceModel.GetGenericTypeArgumentsString()}"
                      : $"{results.GetValue(ResultNames.Namespace).ToString().AppendWhenNotNullOrEmpty(".")}{results.GetValue(ResultNames.BuilderName)}{context.Request.SourceModel.GetGenericTypeArgumentsString()}")
                .AddParameters(context.Request.CreateParameterForBuilder(property, results.GetValue(ResultNames.TypeName)))
                .Chain(method => context.Request.AddNullChecks(method, results))
                .AddCodeStatements
                (
                    results.GetValue(ResultNames.BuilderWithExpression),
                    context.Request.ReturnValueStatementForFluentMethod
                )
            );

            //TODO: Add functionality to GenericFormattableString, so we can compare them by value (just like string)
            if (results.GetValue(ResultNames.TypeName).ToString() != results.GetValue(ResultNames.NonLazyTypeName).ToString())
            {
                //Add overload for non-func type
                context.Request.Builder.AddMethods(new MethodBuilder()
                    .WithName(results.GetValue("MethodName"))
                    .WithReturnTypeName(context.Request.IsBuilderForAbstractEntity
                          ? $"TBuilder{context.Request.SourceModel.GetGenericTypeArgumentsString()}"
                          : $"{results.GetValue(ResultNames.Namespace).ToString().AppendWhenNotNullOrEmpty(".")}{results.GetValue(ResultNames.BuilderName)}{context.Request.SourceModel.GetGenericTypeArgumentsString()}")
                    .AddParameters(context.Request.CreateParameterForBuilder(property, results.GetValue(ResultNames.NonLazyTypeName)))
                    .Chain(method => context.Request.AddNullChecks(method, results))
                    .AddCodeStatements
                    (
                        results.GetValue(ResultNames.BuilderNonLazyWithExpression),
                        context.Request.ReturnValueStatementForFluentMethod
                    )
                );
            }
        }

        return Result.Success();
    }
}

namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class AddExtensionMethodsForNonCollectionPropertiesComponent(IExpressionEvaluator evaluator) : IPipelineComponent<BuilderExtensionContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ProcessAsync(PipelineContext<BuilderExtensionContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (string.IsNullOrEmpty(context.Request.Settings.SetMethodNameFormatString))
        {
            return Result.Success();
        }

        foreach (var property in context.Request.GetSourceProperties()
            .Where(x => !x.TypeName.FixTypeName().IsCollectionTypeName()))
        {
            var parentChildContext = new ParentChildContext<PipelineContext<BuilderExtensionContext>, Property>(context, property, context.Request.Settings);

            var results = await context.Request.GetResultsForBuilderNonCollectionProperties(property, parentChildContext, _evaluator).ConfigureAwait(false);

            var error = results.GetError();
            if (error is not null)
            {
                // Error in formattable string parsing
                return error;
            }

            var returnType = context.Request.GetReturnTypeForFluentMethod(results.GetValue(ResultNames.Namespace), results.GetValue(ResultNames.BuilderName));

            context.Request.Builder.AddMethods(new MethodBuilder()
                .WithName(results.GetValue("MethodName"))
                .WithReturnTypeName("T")
                .WithStatic()
                .WithExtensionMethod()
                .AddGenericTypeArguments("T")
                .AddGenericTypeArgumentConstraints($"where T : {returnType}")
                .AddParameter("instance", "T")
                .AddParameters(context.Request.CreateParameterForBuilder(property, results.GetValue(ResultNames.TypeName)))
                .Chain(method => context.Request.AddNullChecks(method, results))
                .AddCodeStatements
                (
                    results.GetValue(ResultNames.BuilderWithExpression),
                    "return instance;"
                )
            );

            if (context.Request.NeedNonLazyOverloads(results))
            {
                //Add overload for non-func type
                context.Request.Builder.AddMethods(new MethodBuilder()
                    .WithName(results.GetValue("MethodName"))
                    .WithReturnTypeName("T")
                    .WithStatic()
                    .WithExtensionMethod()
                    .AddGenericTypeArguments("T")
                    .AddGenericTypeArgumentConstraints($"where T : {returnType}")
                    .AddParameter("instance", "T")
                    .AddParameters(context.Request.CreateParameterForBuilder(property, results.GetValue(ResultNames.NonLazyTypeName)))
                    .Chain(method => context.Request.AddNullChecks(method, results))
                    .AddCodeStatements
                    (
                        results.GetValue(ResultNames.BuilderNonLazyWithExpression),
                        "return instance;"
                    )
                );
            }
        }

        return Result.Success();
    }
}

namespace ClassFramework.Pipelines.Builder.Components;

public class AddImplicitOperatorComponent(IExpressionEvaluator evaluator) : IPipelineComponent<BuilderContext, ClassBuilder>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ExecuteAsync(BuilderContext context, ClassBuilder response, ICommandService commandService, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));
        response = response.IsNotNull(nameof(response));

        if (!context.Settings.AddImplicitOperatorOnBuilder)
        {
            return Result.Continue();
        }

        if (context.BuildReturnTypeName.GetNamespaceWithDefault().EndsWithAny(".Contracts", ".Abstractions"))
        {
            // Implicit operators are not supported on interfaces (until maybe some future version of C#)
            return Result.Continue();
        }

        return (await _evaluator.EvaluateInterpolatedStringAsync(context.Settings.BuilderNameFormatString, context.FormatProvider, context, token)
            .ConfigureAwait(false))
            .OnSuccess(nameResult =>
            {
                if (context.Settings.EnableBuilderInheritance && context.Settings.IsAbstract)
                {
                    var genericArguments = GetGenericArgumentsForInheritance(context);

                    response.AddMethods(new MethodBuilder()
                        .WithOperator()
                        .WithStatic()
                        .WithName($"{context.BuildReturnTypeName}{context.SourceModel.GetGenericTypeArgumentsString()}")
                        .WithReturnTypeName("implicit")
                        .AddParameter("builder", $"{nameResult.Value}{genericArguments}")
                        .AddCodeStatements(!context.Settings.IsForAbstractBuilder
                            ? "return builder.BuildTyped();"
                            : "return builder.Build();"));

                    return;
                }

                var genericArgumentsString = context.SourceModel.GenericTypeArguments.Count > 0
                    ? context.SourceModel.GetGenericTypeArgumentsString()
                    : string.Empty;

                response.AddMethods(new MethodBuilder()
                    .WithOperator()
                    .WithStatic()
                    .WithName($"{context.BuildReturnTypeName}{context.SourceModel.GetGenericTypeArgumentsString()}")
                    .WithReturnTypeName("implicit")
                    .AddParameter("builder", $"{nameResult.Value}{genericArgumentsString}")
                    .AddCodeStatements($"return builder.{GetName(context)}();"));
                    });
    }

    private static string GetGenericArgumentsForInheritance(BuilderContext context)
    {
        if (context.Settings.IsForAbstractBuilder)
        {
            // This is the non-generic abstract builder
            return string.Empty;
        }

        // This is the generic abstract builder
        return context.SourceModel.GenericTypeArguments.Count > 0
            ? "<TBuilder, TEntity, " + string.Join(", ", context.SourceModel.GenericTypeArguments) + ">"
            : "<TBuilder, TEntity>";
    }

    private static string GetName(BuilderContext context)
        => context.IsBuilderForAbstractEntity || context.IsBuilderForOverrideEntity
            ? context.Settings.BuildTypedMethodName
            : context.Settings.BuildMethodName;
}

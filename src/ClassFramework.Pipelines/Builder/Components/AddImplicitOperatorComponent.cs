namespace ClassFramework.Pipelines.Builder.Components;

public class AddImplicitOperatorComponent(IExpressionEvaluator evaluator) : IPipelineComponent<BuilderContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ProcessAsync(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.AddImplicitOperatorOnBuilder)
        {
            return Result.Success();
        }

        if (context.Request.BuildReturnTypeName.GetNamespaceWithDefault().EndsWithAny(".Contracts", ".Abstractions"))
        {
            // Implicit operators are not supported on interfaces (until maybe some future version of C#)
            return Result.Success();
        }

        var nameResult = await _evaluator.EvaluateInterpolatedStringAsync(context.Request.Settings.BuilderNameFormatString, context.Request.FormatProvider, context.Request, token).ConfigureAwait(false);
        if (!nameResult.IsSuccessful())
        {
            return nameResult;
        }

        if (context.Request.Settings.EnableBuilderInheritance && context.Request.Settings.IsAbstract)
        {
            var genericArguments = GetGenericArgumentsForInheritance(context);

            context.Request.Builder.AddMethods(new MethodBuilder()
                .WithOperator()
                .WithStatic()
                .WithName($"{context.Request.BuildReturnTypeName}{context.Request.SourceModel.GetGenericTypeArgumentsString()}")
                .WithReturnTypeName("implicit")
                .AddParameter("builder", $"{nameResult.Value}{genericArguments}")
                .AddCodeStatements(!context.Request.Settings.IsForAbstractBuilder
                    ? "return builder.BuildTyped();"
                    : "return builder.Build();"));

            return Result.Success();
        }

        var genericArgumentsString = context.Request.SourceModel.GenericTypeArguments.Count > 0
            ? context.Request.SourceModel.GetGenericTypeArgumentsString()
            : string.Empty;

        context.Request.Builder.AddMethods(new MethodBuilder()
            .WithOperator()
            .WithStatic()
            .WithName($"{context.Request.BuildReturnTypeName}{context.Request.SourceModel.GetGenericTypeArgumentsString()}")
            .WithReturnTypeName("implicit")
            .AddParameter("builder", $"{nameResult.Value}{genericArgumentsString}")
            .AddCodeStatements($"return builder.{GetName(context)}();"));

        return Result.Success();
    }

    private static string GetGenericArgumentsForInheritance(PipelineContext<BuilderContext> context)
    {
        if (context.Request.Settings.IsForAbstractBuilder)
        {
            // This is the non-generic abstract builder
            return string.Empty;
        }

        // This is the generic abstract builder
        return context.Request.SourceModel.GenericTypeArguments.Count > 0
            ? "<TBuilder, TEntity, " + string.Join(", ", context.Request.SourceModel.GenericTypeArguments) + ">"
            : "<TBuilder, TEntity>";
    }

    private static string GetName(PipelineContext<BuilderContext> context)
        => context.Request.IsBuilderForAbstractEntity || context.Request.IsBuilderForOverrideEntity
            ? context.Request.Settings.BuildTypedMethodName
            : context.Request.Settings.BuildMethodName;
}

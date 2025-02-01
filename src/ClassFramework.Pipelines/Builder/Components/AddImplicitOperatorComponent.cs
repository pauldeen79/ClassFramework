namespace ClassFramework.Pipelines.Builder.Components;

public class AddImplicitOperatorComponent(IFormattableStringParser formattableStringParser) : IPipelineComponent<BuilderContext>
{
    private readonly IFormattableStringParser _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

    public Task<Result> ProcessAsync(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.AddImplicitOperatorOnBuilder)
        {
            return Task.FromResult(Result.Success());
        }

        if (context.Request.ReturnType.GetNamespaceWithDefault().EndsWithAny(".Contracts", ".Abstractions"))
        {
            // Implicit operators are not supported on interfaces (until maybe some future version of C#)
            return Task.FromResult(Result.Success());
        }

        var nameResult = _formattableStringParser.Parse(context.Request.Settings.BuilderNameFormatString, context.Request.FormatProvider, context.Request);
        if (!nameResult.IsSuccessful())
        {
            return Task.FromResult((Result)nameResult);
        }

        if (context.Request.Settings.EnableBuilderInheritance && context.Request.Settings.IsAbstract)
        {
            var genericArguments = GetGenericArgumentsForInheritance(context);

            context.Request.Builder.AddMethods(new MethodBuilder()
                .WithOperator()
                .WithStatic()
                .WithName(context.Request.ReturnType)
                .WithReturnTypeName("implicit")
                .AddParameter("entity", $"{nameResult.Value}{genericArguments}")
                .AddStringCodeStatements(!context.Request.Settings.IsForAbstractBuilder
                    ? "return entity.BuildTyped();"
                    : "return entity.Build();"));

            return Task.FromResult(Result.Success());
        }

        var genericArgumentsFlat = context.Request.SourceModel.GenericTypeArguments.Count > 0
            ? "<" + string.Join(", ", context.Request.SourceModel.GenericTypeArguments) + ">"
            : string.Empty;

        context.Request.Builder.AddMethods(new MethodBuilder()
            .WithOperator()
            .WithStatic()
            .WithName(context.Request.ReturnType)
            .WithReturnTypeName("implicit")
            .AddParameter("entity", $"{nameResult.Value}{genericArgumentsFlat}")
            .AddStringCodeStatements($"return entity.{GetName(context)}();"));

        return Task.FromResult(Result.Success());
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

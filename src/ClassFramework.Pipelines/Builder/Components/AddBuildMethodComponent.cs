namespace ClassFramework.Pipelines.Builder.Components;

public class AddBuildMethodComponent(IFormattableStringParser formattableStringParser, ICsharpExpressionDumper csharpExpressionDumper) : IPipelineComponent<BuilderContext>
{
    private readonly IFormattableStringParser _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    private readonly ICsharpExpressionDumper _csharpExpressionDumper = csharpExpressionDumper.IsNotNull(nameof(csharpExpressionDumper));

    public Task<Result> ProcessAsync(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        var nameResult = _formattableStringParser.Parse(context.Request.Settings.BuilderNameFormatString, context.Request.FormatProvider, context.Request);
        if (!nameResult.IsSuccessful())
        {
            return Task.FromResult((Result)nameResult);
        }

        if (context.Request.Settings.EnableBuilderInheritance && context.Request.Settings.IsAbstract)
        {
            if (context.Request.Settings.IsForAbstractBuilder)
            {
                context.Request.Builder.AddMethods(new MethodBuilder()
                    .WithName("Build")
                    .WithAbstract()
                    .WithReturnTypeName(context.Request.ReturnType));
            }
            else
            {
                context.Request.Builder.AddMethods(new MethodBuilder()
                    .WithName("Build")
                    .WithOverride()
                    .WithReturnTypeName(context.Request.ReturnType)
                    .AddStringCodeStatements("return BuildTyped();"));

                context.Request.Builder.AddMethods(new MethodBuilder()
                    .WithName("BuildTyped")
                    .WithAbstract()
                    .WithReturnTypeName("TEntity"));
            }

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

        var instanciationResult = context.CreateEntityInstanciation(_formattableStringParser, _csharpExpressionDumper, string.Empty);
        if (!instanciationResult.IsSuccessful())
        {
            return Task.FromResult<Result>(instanciationResult);
        }

        context.Request.Builder.AddMethods(new MethodBuilder()
            .WithName(GetName(context))
            .WithAbstract(context.Request.IsBuilderForAbstractEntity)
            .WithOverride(context.Request.IsBuilderForOverrideEntity)
            .WithReturnTypeName($"{GetBuilderBuildMethodReturnType(context.Request, context.Request.ReturnType)}")
            .AddStringCodeStatements(context.Request.CreatePragmaWarningDisableStatementsForBuildMethod())
            .AddStringCodeStatements
            (
                context.Request.IsBuilderForAbstractEntity
                    ? []
                    : [$"return {instanciationResult.Value};"]
            )
            .AddStringCodeStatements(context.Request.CreatePragmaWarningRestoreStatementsForBuildMethod()));

        if (context.Request.IsBuilderForAbstractEntity)
        {
            var baseClass = context.Request.Settings.BaseClass ?? context.Request.SourceModel;
            context.Request.Builder.AddMethods(new MethodBuilder()
                .WithName(context.Request.Settings.BuildMethodName)
                .WithOverride()
                .WithReturnTypeName($"{baseClass.GetFullName()}{baseClass.GetGenericTypeArgumentsString()}")
                .AddStringCodeStatements($"return {context.Request.Settings.BuildTypedMethodName}();"));
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

    private static string GetBuilderBuildMethodReturnType(BuilderContext context, string returnType)
        => context.IsBuilderForAbstractEntity
            ? "TEntity"
            : returnType;
}

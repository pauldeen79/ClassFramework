namespace ClassFramework.Pipelines.Builder.Components;

public class AddBuildMethodComponent(IExpressionEvaluator evaluator, ICsharpExpressionDumper csharpExpressionDumper) : IPipelineComponent<BuilderContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));
    private readonly ICsharpExpressionDumper _csharpExpressionDumper = csharpExpressionDumper.IsNotNull(nameof(csharpExpressionDumper));

    public async Task<Result> ProcessAsync(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Request.Settings.EnableBuilderInheritance && context.Request.Settings.IsAbstract)
        {
            if (context.Request.Settings.IsForAbstractBuilder)
            {
                context.Request.Builder.AddMethods(new MethodBuilder()
                    .WithName(context.Request.Settings.BuildMethodName)
                    .WithAbstract()
                    .WithReturnTypeName(context.Request.BuildReturnTypeName)
                    .AddReturnTypeGenericTypeArguments(context.Request.SourceModel.GenericTypeArguments.Select(x => new PropertyBuilder().WithName("Dummy").WithTypeName(x))));
            }
            else
            {
                context.Request.Builder.AddMethods(new MethodBuilder()
                    .WithName(context.Request.Settings.BuildMethodName)
                    .WithOverride()
                    .WithReturnTypeName(context.Request.BuildReturnTypeName)
                    .AddReturnTypeGenericTypeArguments(context.Request.SourceModel.GenericTypeArguments.Select(x => new PropertyBuilder().WithName("Dummy").WithTypeName(x)))
                    .AddCodeStatements($"return {context.Request.Settings.BuildTypedMethodName}();"));

                context.Request.Builder.AddMethods(new MethodBuilder()
                    .WithName(context.Request.Settings.BuildTypedMethodName)
                    .WithAbstract()
                    .WithReturnTypeName("TEntity"));
            }

            return await AddExplicitInterfaceImplementations(context, token).ConfigureAwait(false);
        }

        var instanciationResult = await context.CreateEntityInstanciationAsync(_evaluator, _csharpExpressionDumper, string.Empty, token).ConfigureAwait(false);
        if (!instanciationResult.IsSuccessful())
        {
            return instanciationResult;
        }

        context.Request.Builder.AddMethods(new MethodBuilder()
            .WithName(GetName(context))
            .WithAbstract(context.Request.IsBuilderForAbstractEntity)
            .WithOverride(context.Request.IsBuilderForOverrideEntity)
            .WithReturnTypeName(GetBuilderBuildMethodReturnType(context.Request, context.Request.BuildReturnTypeName))
            .AddReturnTypeGenericTypeArguments(context.Request.SourceModel.GenericTypeArguments.Select(x => new PropertyBuilder().WithName("Dummy").WithTypeName(x)))
            .AddCodeStatements(context.Request.CreatePragmaWarningDisableStatementsForBuildMethod())
            .AddCodeStatements
            (
                context.Request.IsBuilderForAbstractEntity
                    ? []
                    : [$"return {instanciationResult.Value};"]
            )
            .AddCodeStatements(context.Request.CreatePragmaWarningRestoreStatementsForBuildMethod()));

        if (context.Request.IsBuilderForAbstractEntity)
        {
            var baseClass = context.Request.Settings.BaseClass ?? context.Request.SourceModel;
            context.Request.Builder.AddMethods(new MethodBuilder()
                .WithName(context.Request.Settings.BuildMethodName)
                .WithOverride()
                .WithReturnTypeName($"{baseClass.GetFullName()}{baseClass.GetGenericTypeArgumentsString()}")
                .AddCodeStatements($"return {context.Request.Settings.BuildTypedMethodName}();"));
        }

        return await AddExplicitInterfaceImplementations(context, token).ConfigureAwait(false);
    }

    private async Task<Result> AddExplicitInterfaceImplementations(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        if (!context.Request.Settings.UseBuilderAbstractionsTypeConversion)
        {
            return Result.Success();
        }

        var interfaces = await context.Request.GetInterfaceResultsAsync(
            (x, y) => new { EntityName = x, BuilderName = context.Request.Settings.UseCrossCuttingInterfaces
                ? typeof(IBuilder<object>).ReplaceGenericTypeName(x)
                : y.ToString() },
            x => new { EntityName = x, BuilderName = context.Request.Settings.UseCrossCuttingInterfaces
                ? typeof(IBuilder<object>).ReplaceGenericTypeName(x)
                : context.Request.MapTypeName(x.FixTypeName()) },
            _evaluator,
            false,
            token).ConfigureAwait(false);

        var error = Array.Find(interfaces, x => !x.IsSuccessful());
        if (error is not null)
        {
            return error;
        }

        var methodName = context.Request.Settings.EnableBuilderInheritance
            && context.Request.Settings.IsAbstract
            && context.Request.Settings.IsForAbstractBuilder
                ? context.Request.Settings.BuildMethodName
                : GetName(context);

        context.Request.Builder.AddMethods(interfaces.Select(x => new MethodBuilder()
            .WithName(context.Request.Settings.BuildMethodName)
            .WithReturnTypeName(x.Value!.EntityName)
            .WithExplicitInterfaceName(x.Value!.BuilderName)
            .AddCodeStatements($"return {methodName}();")));

        return Result.Success();
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

namespace ClassFramework.Pipelines.Builder.Components;

public class AddBuildMethodComponent(IExpressionEvaluator evaluator, ICsharpExpressionDumper csharpExpressionDumper) : IPipelineComponent<BuilderContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));
    private readonly ICsharpExpressionDumper _csharpExpressionDumper = csharpExpressionDumper.IsNotNull(nameof(csharpExpressionDumper));

    public Task<Result> ProcessAsync(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Request.Settings.EnableBuilderInheritance && context.Request.Settings.IsAbstract)
        {
            if (context.Request.Settings.IsForAbstractBuilder)
            {
                context.Request.Builder.AddMethods(new MethodBuilder()
                    .WithName(context.Request.Settings.BuildMethodName)
                    .WithAbstract()
                    .WithReturnTypeName(context.Request.ReturnType)
                    .AddReturnTypeGenericTypeArguments(context.Request.SourceModel.GenericTypeArguments.Select(x => new PropertyBuilder().WithName("Dummy").WithTypeName(x))));
            }
            else
            {
                context.Request.Builder.AddMethods(new MethodBuilder()
                    .WithName(context.Request.Settings.BuildMethodName)
                    .WithOverride()
                    .WithReturnTypeName(context.Request.ReturnType)
                    .AddReturnTypeGenericTypeArguments(context.Request.SourceModel.GenericTypeArguments.Select(x => new PropertyBuilder().WithName("Dummy").WithTypeName(x)))
                    .AddStringCodeStatements($"return {context.Request.Settings.BuildTypedMethodName}();"));

                context.Request.Builder.AddMethods(new MethodBuilder()
                    .WithName(context.Request.Settings.BuildTypedMethodName)
                    .WithAbstract()
                    .WithReturnTypeName("TEntity"));
            }

            return Task.FromResult(AddExplicitInterfaceImplementations(context));
        }

        var instanciationResult = context.CreateEntityInstanciation(_evaluator, _csharpExpressionDumper, string.Empty);
        if (!instanciationResult.IsSuccessful())
        {
            return Task.FromResult<Result>(instanciationResult);
        }

        context.Request.Builder.AddMethods(new MethodBuilder()
            .WithName(GetName(context))
            .WithAbstract(context.Request.IsBuilderForAbstractEntity)
            .WithOverride(context.Request.IsBuilderForOverrideEntity)
            .WithReturnTypeName(GetBuilderBuildMethodReturnType(context.Request, context.Request.ReturnType))
            .AddReturnTypeGenericTypeArguments(context.Request.SourceModel.GenericTypeArguments.Select(x => new PropertyBuilder().WithName("Dummy").WithTypeName(x)))
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

        return Task.FromResult(AddExplicitInterfaceImplementations(context));
    }

    private Result AddExplicitInterfaceImplementations(PipelineContext<BuilderContext> context)
    {
        if (!context.Request.Settings.UseBuilderAbstractionsTypeConversion)
        {
            return Result.Continue();
        }

        var interfaces = context.Request.GetInterfaceResults(
            (x, y) => new { EntityName = x, BuilderName = y.ToString() },
            x => new { EntityName = x, BuilderName = context.Request.MapTypeName(x.FixTypeName()) },
            _evaluator,
            false);

        var error = Array.Find(interfaces, x => !x.IsSuccessful());
        if (error is not null)
        {
            return error;
        }

        var methodName = context.Request.Settings.EnableBuilderInheritance && context.Request.Settings.IsAbstract && context.Request.Settings.IsForAbstractBuilder
            ? context.Request.Settings.BuildMethodName
            : GetName(context);

        context.Request.Builder.AddMethods(interfaces.Select(x => new MethodBuilder()
            .WithName(context.Request.Settings.BuildMethodName)
            .WithReturnTypeName(x.Value!.EntityName)
            .WithExplicitInterfaceName(x.Value!.BuilderName)
            .AddStringCodeStatements($"return {methodName}();")));

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

namespace ClassFramework.Pipelines.Builder.Components;

public class AddBuildMethodComponent(IExpressionEvaluator evaluator, ICsharpExpressionDumper csharpExpressionDumper) : IPipelineComponent<GenerateBuilderCommand, ClassBuilder>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));
    private readonly ICsharpExpressionDumper _csharpExpressionDumper = csharpExpressionDumper.IsNotNull(nameof(csharpExpressionDumper));

    public async Task<Result> ExecuteAsync(GenerateBuilderCommand context, ClassBuilder response, ICommandService commandService, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));
        response = response.IsNotNull(nameof(response));

        if (context.Settings.EnableBuilderInheritance && context.Settings.IsAbstract)
        {
            if (context.Settings.IsForAbstractBuilder)
            {
                response.AddMethods(new MethodBuilder()
                    .WithName(context.Settings.BuildMethodName)
                    .WithAbstract()
                    .WithReturnTypeName(context.BuildReturnTypeName)
                    .AddReturnTypeGenericTypeArguments(context.SourceModel.GenericTypeArguments.Select(x => new PropertyBuilder().WithName("Dummy").WithTypeName(x))));
            }
            else
            {
                response.AddMethods(new MethodBuilder()
                    .WithName(context.Settings.BuildMethodName)
                    .WithOverride()
                    .WithReturnTypeName(context.BuildReturnTypeName)
                    .AddReturnTypeGenericTypeArguments(context.SourceModel.GenericTypeArguments.Select(x => new PropertyBuilder().WithName("Dummy").WithTypeName(x)))
                    .AddCodeStatements($"return {context.Settings.BuildTypedMethodName}();"));

                response.AddMethods(new MethodBuilder()
                    .WithName(context.Settings.BuildTypedMethodName)
                    .WithAbstract()
                    .WithReturnTypeName("TEntity"));
            }

            return await AddExplicitInterfaceImplementations(context, response, token).ConfigureAwait(false);
        }

        var instanciationResult = await context.CreateEntityInstanciationAsync(_evaluator, _csharpExpressionDumper, string.Empty, token).ConfigureAwait(false);
        if (!instanciationResult.IsSuccessful())
        {
            return instanciationResult;
        }

        response.AddMethods(new MethodBuilder()
            .WithName(GetName(context))
            .WithAbstract(context.IsBuilderForAbstractEntity)
            .WithOverride(context.IsBuilderForOverrideEntity)
            .WithReturnTypeName(GetBuilderBuildMethodReturnType(context, context.BuildReturnTypeName))
            .AddReturnTypeGenericTypeArguments(context.SourceModel.GenericTypeArguments.Select(x => new PropertyBuilder().WithName("Dummy").WithTypeName(x)))
            .AddCodeStatements(context.CreatePragmaWarningDisableStatementsForBuildMethod())
            .AddCodeStatements
            (
                context.IsBuilderForAbstractEntity
                    ? []
                    : [$"return {instanciationResult.Value};"]
            )
            .AddCodeStatements(context.CreatePragmaWarningRestoreStatementsForBuildMethod()));

        if (context.IsBuilderForAbstractEntity)
        {
            var baseClass = context.Settings.BaseClass ?? context.SourceModel;
            response.AddMethods(new MethodBuilder()
                .WithName(context.Settings.BuildMethodName)
                .WithOverride()
                .WithReturnTypeName($"{baseClass.GetFullName()}{baseClass.GetGenericTypeArgumentsString()}")
                .AddCodeStatements($"return {context.Settings.BuildTypedMethodName}();"));
        }

        return await AddExplicitInterfaceImplementations(context, response, token).ConfigureAwait(false);
    }

    private async Task<Result> AddExplicitInterfaceImplementations(GenerateBuilderCommand context, ClassBuilder response, CancellationToken token)
    {
        if (!context.Settings.UseBuilderAbstractionsTypeConversion)
        {
            return Result.Success();
        }

        var interfaces = await context.GetInterfaceResultsAsync(
            (x, y) => new { EntityName = x, BuilderName = context.Settings.UseCrossCuttingInterfaces
                ? typeof(IBuilder<object>).ReplaceGenericTypeName(x)
                : y.ToString() },
            x => new { EntityName = x, BuilderName = context.Settings.UseCrossCuttingInterfaces
                ? typeof(IBuilder<object>).ReplaceGenericTypeName(x)
                : context.MapTypeName(x.FixTypeName()) },
            _evaluator,
            false,
            token).ConfigureAwait(false);

        var error = Array.Find(interfaces, x => !x.IsSuccessful());
        if (error is not null)
        {
            return error;
        }

        var methodName = context.Settings.EnableBuilderInheritance
            && context.Settings.IsAbstract
            && context.Settings.IsForAbstractBuilder
                ? context.Settings.BuildMethodName
                : GetName(context);

        response.AddMethods(interfaces.Select(x => new MethodBuilder()
            .WithName(context.Settings.BuildMethodName)
            .WithReturnTypeName(x.Value!.EntityName)
            .WithExplicitInterfaceName(x.Value!.BuilderName)
            .AddCodeStatements($"return {methodName}();")));

        return Result.Success();
    }

    private static string GetName(GenerateBuilderCommand context)
        => context.IsBuilderForAbstractEntity || context.IsBuilderForOverrideEntity
            ? context.Settings.BuildTypedMethodName
            : context.Settings.BuildMethodName;

    private static string GetBuilderBuildMethodReturnType(GenerateBuilderCommand context, string returnType)
        => context.IsBuilderForAbstractEntity
            ? "TEntity"
            : returnType;
}

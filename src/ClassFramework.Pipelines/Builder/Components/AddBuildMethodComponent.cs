namespace ClassFramework.Pipelines.Builder.Components;

public class AddBuildMethodComponent(IExpressionEvaluator evaluator, ICsharpExpressionDumper csharpExpressionDumper) : IPipelineComponent<GenerateBuilderCommand, ClassBuilder>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));
    private readonly ICsharpExpressionDumper _csharpExpressionDumper = csharpExpressionDumper.IsNotNull(nameof(csharpExpressionDumper));

    public async Task<Result> ExecuteAsync(GenerateBuilderCommand command, ClassBuilder response, ICommandService commandService, CancellationToken token)
    {
        command = command.IsNotNull(nameof(command));
        response = response.IsNotNull(nameof(response));

        if (command.Settings.EnableBuilderInheritance && command.Settings.IsAbstract)
        {
            if (command.Settings.IsForAbstractBuilder)
            {
                response.AddMethods(new MethodBuilder()
                    .WithName(command.Settings.BuildMethodName)
                    .WithAbstract()
                    .WithReturnTypeName(command.BuildReturnTypeName)
                    .AddReturnTypeGenericTypeArguments(command.SourceModel.GenericTypeArguments.Select(x => new PropertyBuilder().WithName("Dummy").WithTypeName(x))));
            }
            else
            {
                response.AddMethods(new MethodBuilder()
                    .WithName(command.Settings.BuildMethodName)
                    .WithOverride()
                    .WithReturnTypeName(command.BuildReturnTypeName)
                    .AddReturnTypeGenericTypeArguments(command.SourceModel.GenericTypeArguments.Select(x => new PropertyBuilder().WithName("Dummy").WithTypeName(x)))
                    .AddCodeStatements($"return {command.Settings.BuildTypedMethodName}();"));

                response.AddMethods(new MethodBuilder()
                    .WithName(command.Settings.BuildTypedMethodName)
                    .WithAbstract()
                    .WithReturnTypeName("TEntity"));
            }

            return await AddExplicitInterfaceImplementationsAsync(command, response, token).ConfigureAwait(false);
        }

        var instanciationResult = await command.CreateEntityInstanciationAsync(_evaluator, _csharpExpressionDumper, string.Empty, token).ConfigureAwait(false);
        if (!instanciationResult.IsSuccessful())
        {
            return instanciationResult;
        }

        response.AddMethods(new MethodBuilder()
            .WithName(GetName(command))
            .WithAbstract(command.IsBuilderForAbstractEntity)
            .WithOverride(command.IsBuilderForOverrideEntity)
            .WithReturnTypeName(GetBuilderBuildMethodReturnType(command, command.BuildReturnTypeName))
            .AddReturnTypeGenericTypeArguments(command.SourceModel.GenericTypeArguments.Select(x => new PropertyBuilder().WithName("Dummy").WithTypeName(x)))
            .AddCodeStatements(command.CreatePragmaWarningDisableStatementsForBuildMethod())
            .AddCodeStatements
            (
                command.IsBuilderForAbstractEntity
                    ? []
                    : [$"return {instanciationResult.Value};"]
            )
            .AddCodeStatements(command.CreatePragmaWarningRestoreStatementsForBuildMethod()));

        if (command.IsBuilderForAbstractEntity)
        {
            var baseClass = command.Settings.BaseClass ?? command.SourceModel;
            response.AddMethods(new MethodBuilder()
                .WithName(command.Settings.BuildMethodName)
                .WithOverride()
                .WithReturnTypeName($"{baseClass.GetFullName()}{baseClass.GetGenericTypeArgumentsString()}")
                .AddCodeStatements($"return {command.Settings.BuildTypedMethodName}();"));
        }

        return await AddExplicitInterfaceImplementationsAsync(command, response, token).ConfigureAwait(false);
    }

    private async Task<Result> AddExplicitInterfaceImplementationsAsync(GenerateBuilderCommand command, ClassBuilder response, CancellationToken token)
    {
        if (!command.Settings.UseBuilderAbstractionsTypeConversion)
        {
            return Result.Success();
        }

        var interfaces = await command.GetInterfaceResultsAsync(
            (x, y) => new { EntityName = x, BuilderName = command.Settings.UseCrossCuttingInterfaces
                ? typeof(IBuilder<object>).ReplaceGenericTypeName(x)
                : y.ToString() },
            x => new { EntityName = x, BuilderName = command.Settings.UseCrossCuttingInterfaces
                ? typeof(IBuilder<object>).ReplaceGenericTypeName(x)
                : command.MapTypeName(x.FixTypeName()) },
            _evaluator,
            false,
            token).ConfigureAwait(false);

        var error = Array.Find(interfaces, x => !x.IsSuccessful());
        if (error is not null)
        {
            return error;
        }

        var methodName = command.Settings.EnableBuilderInheritance
            && command.Settings.IsAbstract
            && command.Settings.IsForAbstractBuilder
                ? command.Settings.BuildMethodName
                : GetName(command);

        response.AddMethods(interfaces.Select(x => new MethodBuilder()
            .WithName(command.Settings.BuildMethodName)
            .WithReturnTypeName(x.Value!.EntityName)
            .WithExplicitInterfaceName(x.Value!.BuilderName)
            .AddCodeStatements($"return {methodName}();")));

        return Result.Success();
    }

    private static string GetName(GenerateBuilderCommand command)
        => command.IsBuilderForAbstractEntity || command.IsBuilderForOverrideEntity
            ? command.Settings.BuildTypedMethodName
            : command.Settings.BuildMethodName;

    private static string GetBuilderBuildMethodReturnType(GenerateBuilderCommand command, string returnType)
        => command.IsBuilderForAbstractEntity
            ? "TEntity"
            : returnType;
}

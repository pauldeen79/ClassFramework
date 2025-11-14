namespace ClassFramework.Pipelines.Builder.Components;

public class AddImplicitOperatorComponent(IExpressionEvaluator evaluator) : IPipelineComponent<GenerateBuilderCommand, ClassBuilder>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ExecuteAsync(GenerateBuilderCommand command, ClassBuilder response, ICommandService commandService, CancellationToken token)
    {
        command = command.IsNotNull(nameof(command));
        response = response.IsNotNull(nameof(response));

        if (!command.Settings.AddImplicitOperatorOnBuilder)
        {
            return Result.Continue();
        }

        if (command.BuildReturnTypeName.GetNamespaceWithDefault().EndsWithAny(".Contracts", ".Abstractions"))
        {
            // Implicit operators are not supported on interfaces (until maybe some future version of C#)
            return Result.Continue();
        }

        return (await _evaluator.EvaluateInterpolatedStringAsync(command.Settings.BuilderNameFormatString, command.FormatProvider, command, token)
            .ConfigureAwait(false))
            .OnSuccess(nameResult =>
            {
                if (command.Settings.EnableBuilderInheritance && command.Settings.IsAbstract)
                {
                    var genericArguments = GetGenericArgumentsForInheritance(command);

                    response.AddMethods(new MethodBuilder()
                        .WithOperator()
                        .WithStatic()
                        .WithName($"{command.BuildReturnTypeName}{command.SourceModel.GetGenericTypeArgumentsString()}")
                        .WithReturnTypeName("implicit")
                        .AddParameter("builder", $"{nameResult.Value}{genericArguments}")
                        .AddCodeStatements(!command.Settings.IsForAbstractBuilder
                            ? "return builder.BuildTyped();"
                            : "return builder.Build();"));

                    return;
                }

                var genericArgumentsString = command.SourceModel.GenericTypeArguments.Count > 0
                    ? command.SourceModel.GetGenericTypeArgumentsString()
                    : string.Empty;

                response.AddMethods(new MethodBuilder()
                    .WithOperator()
                    .WithStatic()
                    .WithName($"{command.BuildReturnTypeName}{command.SourceModel.GetGenericTypeArgumentsString()}")
                    .WithReturnTypeName("implicit")
                    .AddParameter("builder", $"{nameResult.Value}{genericArgumentsString}")
                    .AddCodeStatements($"return builder.{GetName(command)}();"));
                    });
    }

    private static string GetGenericArgumentsForInheritance(GenerateBuilderCommand command)
    {
        if (command.Settings.IsForAbstractBuilder)
        {
            // This is the non-generic abstract builder
            return string.Empty;
        }

        // This is the generic abstract builder
        return command.SourceModel.GenericTypeArguments.Count > 0
            ? "<TBuilder, TEntity, " + string.Join(", ", command.SourceModel.GenericTypeArguments) + ">"
            : "<TBuilder, TEntity>";
    }

    private static string GetName(GenerateBuilderCommand command)
        => command.IsBuilderForAbstractEntity || command.IsBuilderForOverrideEntity
            ? command.Settings.BuildTypedMethodName
            : command.Settings.BuildMethodName;
}

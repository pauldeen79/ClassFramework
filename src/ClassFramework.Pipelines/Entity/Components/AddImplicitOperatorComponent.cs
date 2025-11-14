namespace ClassFramework.Pipelines.Entity.Components;

public class AddImplicitOperatorComponent(IExpressionEvaluator evaluator) : IPipelineComponent<GenerateEntityCommand, ClassBuilder>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ExecuteAsync(GenerateEntityCommand command, ClassBuilder response, ICommandService commandService, CancellationToken token)
    {
        command = command.IsNotNull(nameof(command));
        response = response.IsNotNull(nameof(response));

        if (!command.Settings.AddImplicitOperatorOnEntity)
        {
            return Result.Continue();
        }

        return (await command.GetToBuilderResultsAsync(_evaluator, token).ConfigureAwait(false))
            .OnSuccess(results =>
            {
                var methodName = results.GetValue("ToBuilderMethodName");
                if (string.IsNullOrEmpty(methodName))
                {
                    return Result.Continue();
                }

                return command.GetCustomNamespaceResults(results)
                    .OnSuccess(customNamespaceResults =>
                    {
                        var builderConcreteName = customNamespaceResults.GetValue("BuilderConcreteName");
                        var generics = command.SourceModel.GetGenericTypeArgumentsString();
                        var builderName = results.GetValue(ResultNames.BuilderName).ToString().Replace(command.SourceModel.Name, builderConcreteName);
                        var builderConcreteTypeName = $"{customNamespaceResults.GetValue("CustomBuilderNamespace")}.{builderName}";
                        var builderTypeName = command.GetBuilderTypeName(customNamespaceResults.GetValue("CustomBuilderInterfaceNamespace"), customNamespaceResults.GetValue("CustomConcreteBuilderNamespace"), builderConcreteName, builderConcreteTypeName, results.GetValue(ResultNames.BuilderName));
                        var entityFullName = command.GetEntityFullName(results.GetValue(ResultNames.Namespace).ToString(), results.GetValue(ResultNames.Name).ToString());
                        var typedMethodName = results.GetValue("ToTypedBuilderMethodName");

                        var operatorMethod = new MethodBuilder()
                                .WithOperator()
                                .WithStatic()
                                .WithReturnTypeName("implicit")
                                .AddParameter("entity", $"{entityFullName}{generics}");

                        if (command.Settings.EnableInheritance
                            && command.Settings.BaseClass is not null
                            && !string.IsNullOrEmpty(typedMethodName))
                        {
                            response.AddMethods(operatorMethod
                                .WithName($"{builderConcreteTypeName}{generics}")
                                .AddCodeStatements($"return entity.{typedMethodName}();"));
                        }
                        else
                        {
                            response.AddMethods(operatorMethod
                                .WithName($"{builderTypeName}{generics}")
                                .AddCodeStatements($"return entity.{methodName}();"));
                        }
                    });
            });
    }
}

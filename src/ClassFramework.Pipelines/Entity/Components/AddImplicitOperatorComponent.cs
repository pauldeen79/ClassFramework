namespace ClassFramework.Pipelines.Entity.Components;

public class AddImplicitOperatorComponent(IExpressionEvaluator evaluator) : IPipelineComponent<EntityContext, ClassBuilder>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ExecuteAsync(EntityContext context, ClassBuilder response, ICommandService commandService, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));
        response = response.IsNotNull(nameof(response));

        if (!context.Settings.AddImplicitOperatorOnEntity)
        {
            return Result.Continue();
        }

        return (await context.GetToBuilderResultsAsync(_evaluator, token).ConfigureAwait(false))
            .OnSuccess(results =>
            {
                var methodName = results.GetValue("ToBuilderMethodName");
                if (string.IsNullOrEmpty(methodName))
                {
                    return Result.Continue();
                }

                return context.GetCustomNamespaceResults(results)
                    .OnSuccess(customNamespaceResults =>
                    {
                        var builderConcreteName = customNamespaceResults.GetValue("BuilderConcreteName");
                        var generics = context.SourceModel.GetGenericTypeArgumentsString();
                        var builderName = results.GetValue(ResultNames.BuilderName).ToString().Replace(context.SourceModel.Name, builderConcreteName);
                        var builderConcreteTypeName = $"{customNamespaceResults.GetValue("CustomBuilderNamespace")}.{builderName}";
                        var builderTypeName = context.GetBuilderTypeName(customNamespaceResults.GetValue("CustomBuilderInterfaceNamespace"), customNamespaceResults.GetValue("CustomConcreteBuilderNamespace"), builderConcreteName, builderConcreteTypeName, results.GetValue(ResultNames.BuilderName));
                        var entityFullName = context.GetEntityFullName(results.GetValue(ResultNames.Namespace).ToString(), results.GetValue(ResultNames.Name).ToString());
                        var typedMethodName = results.GetValue("ToTypedBuilderMethodName");

                        var operatorMethod = new MethodBuilder()
                                .WithOperator()
                                .WithStatic()
                                .WithReturnTypeName("implicit")
                                .AddParameter("entity", $"{entityFullName}{generics}");

                        if (context.Settings.EnableInheritance
                            && context.Settings.BaseClass is not null
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

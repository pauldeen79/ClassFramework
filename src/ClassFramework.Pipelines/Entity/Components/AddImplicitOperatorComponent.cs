namespace ClassFramework.Pipelines.Entity.Components;

public class AddImplicitOperatorComponent(IExpressionEvaluator evaluator) : IPipelineComponent<EntityContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ExecuteAsync(EntityContext context, ICommandService commandService, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

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
                            context.Builder.AddMethods(operatorMethod
                                .WithName($"{builderConcreteTypeName}{generics}")
                                .AddCodeStatements($"return entity.{typedMethodName}();"));
                        }
                        else
                        {
                            context.Builder.AddMethods(operatorMethod
                                .WithName($"{builderTypeName}{generics}")
                                .AddCodeStatements($"return entity.{methodName}();"));
                        }
                    });
            });
    }
}

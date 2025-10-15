namespace ClassFramework.Pipelines.Entity.Components;

public class AddImplicitOperatorComponent(IExpressionEvaluator evaluator) : IPipelineComponent<EntityContext>, IOrderContainer
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public int Order => PipelineStage.Process;

    public async Task<Result> ProcessAsync(PipelineContext<EntityContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.AddImplicitOperatorOnEntity)
        {
            return Result.Continue();
        }

        return (await context.Request.GetToBuilderResultsAsync(_evaluator, token).ConfigureAwait(false))
            .OnSuccess(results =>
            {
                var methodName = results.GetValue("ToBuilderMethodName");
                if (string.IsNullOrEmpty(methodName))
                {
                    return Result.Continue();
                }

                return context.Request.GetCustomNamespaceResults(results)
                    .OnSuccess(customNamespaceResults =>
                    {
                        var builderConcreteName = customNamespaceResults.GetValue("BuilderConcreteName");
                        var generics = context.Request.SourceModel.GetGenericTypeArgumentsString();
                        var builderName = results.GetValue(ResultNames.BuilderName).ToString().Replace(context.Request.SourceModel.Name, builderConcreteName);
                        var builderConcreteTypeName = $"{customNamespaceResults.GetValue("CustomBuilderNamespace")}.{builderName}";
                        var builderTypeName = context.Request.GetBuilderTypeName(customNamespaceResults.GetValue("CustomBuilderInterfaceNamespace"), customNamespaceResults.GetValue("CustomConcreteBuilderNamespace"), builderConcreteName, builderConcreteTypeName, results.GetValue(ResultNames.BuilderName));
                        var entityFullName = context.Request.GetEntityFullName(results.GetValue(ResultNames.Namespace).ToString(), results.GetValue(ResultNames.Name).ToString());
                        var typedMethodName = results.GetValue("ToTypedBuilderMethodName");

                        var operatorMethod = new MethodBuilder()
                                .WithOperator()
                                .WithStatic()
                                .WithReturnTypeName("implicit")
                                .AddParameter("entity", $"{entityFullName}{generics}");

                        if (context.Request.Settings.EnableInheritance
                            && context.Request.Settings.BaseClass is not null
                            && !string.IsNullOrEmpty(typedMethodName))
                        {
                            context.Request.Builder.AddMethods(operatorMethod
                                .WithName($"{builderConcreteTypeName}{generics}")
                                .AddCodeStatements($"return entity.{typedMethodName}();"));
                        }
                        else
                        {
                            context.Request.Builder.AddMethods(operatorMethod
                                .WithName($"{builderTypeName}{generics}")
                                .AddCodeStatements($"return entity.{methodName}();"));
                        }
                    });
            });
    }
}

namespace ClassFramework.Pipelines.Entity.Components;

public class AddImplicitOperatorComponent(IExpressionEvaluator evaluator) : IPipelineComponent<EntityContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ProcessAsync(PipelineContext<EntityContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.AddImplicitOperatorOnEntity)
        {
            return Result.Success();
        }

        var results = await context.Request.GetToBuilderResultsAsync(_evaluator, token).ConfigureAwait(false);

        var error = results.GetError();
        if (error is not null)
        {
            // Error in formattable string parsing
            return error;
        }

        var methodName = results.GetValue("ToBuilderMethodName");
        if (string.IsNullOrEmpty(methodName))
        {
            return Result.Success();
        }

        var customNamespaceResults = context.Request.GetCustomNamespaceResults(results);
        var customNamespaceError = customNamespaceResults.GetError();
        if (customNamespaceError is not null)
        {
            // Error in formattable string parsing
            return customNamespaceError;
        }

        var typedMethodName = results.GetValue("ToTypedBuilderMethodName");
        var builderConcreteName = customNamespaceResults.GetValue("BuilderConcreteName");
        var generics = context.Request.SourceModel.GetGenericTypeArgumentsString();
        var builderName = results.GetValue(ResultNames.BuilderName).ToString().Replace(context.Request.SourceModel.Name, builderConcreteName);
        var builderConcreteTypeName = $"{customNamespaceResults.GetValue("CustomBuilderNamespace")}.{builderName}";
        var builderTypeName = context.Request.GetBuilderTypeName(customNamespaceResults.GetValue("CustomBuilderInterfaceNamespace"), customNamespaceResults.GetValue("CustomConcreteBuilderNamespace"), builderConcreteName, builderConcreteTypeName, results.GetValue(ResultNames.BuilderName));
        var name = results.GetValue(ResultNames.Name).ToString();
        var ns = results.GetValue(ResultNames.Namespace).ToString();
        var entityFullName = context.Request.GetEntityFullName(ns, name);

        if (context.Request.Settings.EnableInheritance
            && context.Request.Settings.BaseClass is not null
            && !string.IsNullOrEmpty(typedMethodName))
        {
            context.Request.Builder.AddMethods(new MethodBuilder()
                .WithOperator()
                .WithStatic()
                .WithName($"{builderConcreteTypeName}{generics}")
                .WithReturnTypeName("implicit")
                .AddParameter("entity", $"{entityFullName}{generics}")
                .AddCodeStatements($"return entity.{typedMethodName}();"));
        }
        else
        {
            context.Request.Builder.AddMethods(new MethodBuilder()
                .WithOperator()
                .WithStatic()
                .WithName($"{builderTypeName}{generics}")
                .WithReturnTypeName("implicit")
                .AddParameter("entity", $"{entityFullName}{generics}")
                .AddCodeStatements($"return entity.{methodName}();"));
        }

        return Result.Success();
    }
}

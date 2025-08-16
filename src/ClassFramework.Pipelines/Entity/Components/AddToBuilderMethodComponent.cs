namespace ClassFramework.Pipelines.Entity.Components;

public class AddToBuilderMethodComponent(IExpressionEvaluator evaluator) : IPipelineComponent<EntityContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ProcessAsync(PipelineContext<EntityContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        var results = await context.Request.GetToBuilderResultsAsync(evaluator, token).ConfigureAwait(false);

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

        var typedMethodName = results.GetValue("ToTypedBuilderMethodName");

        var customNamespaceResults = context.Request.GetCustomNamespaceResults(results);
        var customNamespaceError = customNamespaceResults.GetError();
        if (customNamespaceError is not null)
        {
            // Error in formattable string parsing
            return customNamespaceError;
        }

        var builderConcreteName = customNamespaceResults.GetValue("BuilderConcreteName");
        var generics = context.Request.SourceModel.GetGenericTypeArgumentsString();
        var builderName = results.GetValue(ResultNames.BuilderName).ToString().Replace(context.Request.SourceModel.Name, builderConcreteName);
        var builderConcreteTypeName = $"{customNamespaceResults.GetValue("CustomBuilderNamespace")}.{builderName}";
        var builderTypeName = context.Request.GetBuilderTypeName(customNamespaceResults.GetValue("CustomBuilderInterfaceNamespace"), customNamespaceResults.GetValue("CustomConcreteBuilderNamespace"), builderConcreteName, builderConcreteTypeName, results.GetValue(ResultNames.BuilderName));
        var returnStatement = customNamespaceResults.GetValue("ReturnStatement");

        context.Request.Builder
            .AddMethods(new MethodBuilder()
                .WithName(methodName)
                .WithAbstract(context.Request.IsAbstract)
                .WithOverride(context.Request.Settings.BaseClass is not null)
                .WithReturnTypeName(builderTypeName)
                .AddReturnTypeGenericTypeArguments(context.Request.Settings.BaseClass is not null
                    ? Enumerable.Empty<ITypeContainerBuilder>()
                    : context.Request.SourceModel.GenericTypeArguments.Select(x => new PropertyBuilder().WithName("Dummy").WithTypeName(x)))
                .AddCodeStatements(returnStatement));

        if (context.Request.Settings.EnableInheritance
            && context.Request.Settings.BaseClass is not null
            && !string.IsNullOrEmpty(typedMethodName))
        {
            context.Request.Builder
                .AddMethods(new MethodBuilder()
                    .WithName(typedMethodName)
                    .WithReturnTypeName(builderConcreteTypeName)
                    .AddReturnTypeGenericTypeArguments(context.Request.SourceModel.GenericTypeArguments.Select(x => new PropertyBuilder().WithName("Dummy").WithTypeName(x)))
                    .AddCodeStatements($"return new {builderConcreteTypeName}{generics}(this);"));
        }
        else if (context.Request.Settings.UseCrossCuttingInterfaces)
        {
            context.Request.Builder.AddInterfaces(typeof(IBuildableEntity<object>).ReplaceGenericTypeName(builderTypeName));
        }

        return await AddExplicitInterfaceImplementations(context, methodName, typedMethodName, token).ConfigureAwait(false);
    }

    private async Task<Result> AddExplicitInterfaceImplementations(PipelineContext<EntityContext> context, string methodName, string typedMethodName, CancellationToken token)
    {
        if (!context.Request.Settings.UseBuilderAbstractionsTypeConversion)
        {
            return Result.Continue();
        }

        var interfaces = new List<Result<NameInfo>>();

        var sourceInterfaces = context.Request.SourceModel.Interfaces
            .Where(
                x => (context.Request.Settings.CopyInterfacePredicate?.Invoke(x) ?? true)
                  && context.Request.Settings.BuilderAbstractionsTypeConversionNamespaces.Contains(x.GetNamespaceWithDefault()));

        foreach (var x in sourceInterfaces)
        {
            var metadata = context.Request.GetMappingMetadata(x).ToArray();
            var ns = metadata.GetStringValue(MetadataNames.CustomBuilderInterfaceNamespace);

            if (!string.IsNullOrEmpty(ns))
            {
                var property = new PropertyBuilder()
                    .WithName("Dummy")
                    .WithTypeName(x)
                    .Build();
                var newTypeName = metadata.GetStringValue(MetadataNames.CustomBuilderInterfaceName, "I{NoGenerics(ClassName(property.TypeName))}Builder{GenericArguments(property.TypeName, true)}");
                var newFullName = $"{ns}.{newTypeName}";

                interfaces.Add((await _evaluator.EvaluateInterpolatedStringAsync
                (
                    newFullName,
                    context.Request.FormatProvider,
                    new ParentChildContext<PipelineContext<EntityContext>, Property>(context, property, context.Request.Settings),
                    token
                ).ConfigureAwait(false)).Transform(y => new NameInfo(GetEntityName(context.Request.Settings, x, () => y.ToString()), y.ToString())));
            }
            else
            {
                interfaces.Add(Result.Success(new NameInfo(GetEntityName(context.Request.Settings, x, () => context.Request.MapTypeName(x.FixTypeName())), context.Request.MapTypeName(x.FixTypeName()))));
            }
        }

        var error = interfaces.Find(x => !x.IsSuccessful());
        if (error is not null)
        {
            return error;
        }

        var methodCallName = context.Request.Settings.EnableInheritance
            && context.Request.Settings.BaseClass is not null
            && !string.IsNullOrEmpty(typedMethodName)
                ? typedMethodName
                : methodName;

        context.Request.Builder.AddMethods(interfaces.Select(x => new MethodBuilder()
            .WithName(methodName)
            .WithReturnTypeName(x.Value!.BuilderName)
            .WithExplicitInterfaceName(x.Value!.EntityName)
            .AddCodeStatements($"return {methodCallName}();")));

        return Result.Success();
    }

    private static string GetEntityName(PipelineSettings settings, string entityTypeName, Func<string> builderTypeName)
    {
        if (settings.UseCrossCuttingInterfaces)
        {
            return typeof(IBuildableEntity<object>).ReplaceGenericTypeName(builderTypeName());
        }

        return entityTypeName;
    }

    private sealed class NameInfo
    {
        public NameInfo(string entityName, string builderName)
        {
            EntityName = entityName;
            BuilderName = builderName;
        }

        public string EntityName { get; }
        public string BuilderName { get; }
    }
}

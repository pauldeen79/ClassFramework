namespace ClassFramework.Pipelines.Entity.Components;

public class AddToBuilderMethodComponent(IExpressionEvaluator evaluator) : IPipelineComponent<EntityContext>, IOrderContainer
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public int Order => PipelineStage.Process;

    public async Task<Result> ExecuteAsync(EntityContext context, ICommandService commandService, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        return await (await context.GetToBuilderResultsAsync(evaluator, token).ConfigureAwait(false))
            .OnSuccessAsync(async results =>
            {
                var methodName = results.GetValue("ToBuilderMethodName");
                if (string.IsNullOrEmpty(methodName))
                {
                    return Result.Continue();
                }

                return await context.GetCustomNamespaceResults(results)
                    .OnSuccessAsync(async customNamespaceResults =>
                    {
                        var typedMethodName = results.GetValue("ToTypedBuilderMethodName");
                        var builderConcreteName = customNamespaceResults.GetValue("BuilderConcreteName");
                        var generics = context.SourceModel.GetGenericTypeArgumentsString();
                        var builderName = results.GetValue(ResultNames.BuilderName).ToString().Replace(context.SourceModel.Name, builderConcreteName);
                        var builderConcreteTypeName = $"{customNamespaceResults.GetValue("CustomBuilderNamespace")}.{builderName}";
                        var builderTypeName = context.GetBuilderTypeName(customNamespaceResults.GetValue("CustomBuilderInterfaceNamespace"), customNamespaceResults.GetValue("CustomConcreteBuilderNamespace"), builderConcreteName, builderConcreteTypeName, results.GetValue(ResultNames.BuilderName));
                        var returnStatement = customNamespaceResults.GetValue("ReturnStatement");

                        context.Builder
                            .AddMethods(new MethodBuilder()
                                .WithName(methodName)
                                .WithAbstract(context.IsAbstract)
                                .WithOverride(context.Settings.BaseClass is not null)
                                .WithReturnTypeName(builderTypeName)
                                .AddReturnTypeGenericTypeArguments(context.Settings.BaseClass is not null
                                    ? Enumerable.Empty<ITypeContainerBuilder>()
                                    : context.SourceModel.GenericTypeArguments.Select(x => new PropertyBuilder().WithName("Dummy").WithTypeName(x)))
                                .AddCodeStatements(returnStatement));

                        if (context.Settings.EnableInheritance
                            && context.Settings.BaseClass is not null
                            && !string.IsNullOrEmpty(typedMethodName))
                        {
                            context.Builder
                                .AddMethods(new MethodBuilder()
                                    .WithName(typedMethodName)
                                    .WithReturnTypeName(builderConcreteTypeName)
                                    .AddReturnTypeGenericTypeArguments(context.SourceModel.GenericTypeArguments.Select(x => new PropertyBuilder().WithName("Dummy").WithTypeName(x)))
                                    .AddCodeStatements($"return new {builderConcreteTypeName}{generics}(this);"));
                        }
                        else if (context.Settings.UseCrossCuttingInterfaces)
                        {
                            var entityInterface = typeof(IBuildableEntity<object>).ReplaceGenericTypeName(builderTypeName);
                            if (!context.Builder.Interfaces.Contains(entityInterface))
                            {
                                context.Builder.AddInterfaces(entityInterface);
                            }
                        }

                        return await AddExplicitInterfaceImplementations(context, methodName, typedMethodName, token).ConfigureAwait(false);
                    }).ConfigureAwait(false);
            }).ConfigureAwait(false);
    }

    private async Task<Result> AddExplicitInterfaceImplementations(EntityContext context, string methodName, string typedMethodName, CancellationToken token)
    {
        if (!context.Settings.UseBuilderAbstractionsTypeConversion)
        {
            return Result.Success();
        }

        var interfaces = new List<Result<NameInfo>>();

        var sourceInterfaces = context.SourceModel.Interfaces
            .Where(
                x => (context.Settings.CopyInterfacePredicate?.Invoke(x) ?? true)
                  && context.Settings.BuilderAbstractionsTypeConversionNamespaces.Contains(x.GetNamespaceWithDefault()));

        foreach (var sourceInterface in sourceInterfaces)
        {
            var metadata = context.GetMappingMetadata(sourceInterface).ToArray();
            var ns = metadata.GetStringValue(MetadataNames.CustomBuilderInterfaceNamespace);

            if (!string.IsNullOrEmpty(ns))
            {
                var property = new PropertyBuilder()
                    .WithName("Dummy")
                    .WithTypeName(sourceInterface)
                    .Build();
                var newTypeName = metadata.GetStringValue(MetadataNames.CustomBuilderInterfaceName, "I{NoGenerics(ClassName(property.TypeName))}Builder{GenericArguments(property.TypeName, true)}");
                var newFullName = $"{ns}.{newTypeName}";

                interfaces.Add((await _evaluator.EvaluateInterpolatedStringAsync
                (
                    newFullName,
                    context.FormatProvider,
                    new ParentChildContext<EntityContext, Property>(context, property, context.Settings),
                    token
                ).ConfigureAwait(false)).Transform(y => new NameInfo(GetEntityName(context.Settings, sourceInterface, () => y.ToString()), y.ToString())));
            }
            else
            {
                interfaces.Add(Result.Success(new NameInfo(GetEntityName(context.Settings, sourceInterface, () => context.MapTypeName(sourceInterface.FixTypeName())), context.MapTypeName(sourceInterface.FixTypeName()))));
            }
        }

        var error = interfaces.Find(x => !x.IsSuccessful());
        if (error is not null)
        {
            return error;
        }

        var methodCallName = context.Settings.EnableInheritance
            && context.Settings.BaseClass is not null
            && !string.IsNullOrEmpty(typedMethodName)
                ? typedMethodName
                : methodName;

        context.Builder.AddMethods(interfaces.Select(x => new MethodBuilder()
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

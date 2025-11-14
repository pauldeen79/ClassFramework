namespace ClassFramework.Pipelines.Entity.Components;

public class AddToBuilderMethodComponent(IExpressionEvaluator evaluator) : IPipelineComponent<GenerateEntityCommand, ClassBuilder>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ExecuteAsync(GenerateEntityCommand command, ClassBuilder response, ICommandService commandService, CancellationToken token)
    {
        command = command.IsNotNull(nameof(command));
        response = response.IsNotNull(nameof(response));

        return await (await command.GetToBuilderResultsAsync(evaluator, token).ConfigureAwait(false))
            .OnSuccessAsync(async results =>
            {
                var methodName = results.GetValue("ToBuilderMethodName");
                if (string.IsNullOrEmpty(methodName))
                {
                    return Result.Continue();
                }

                return await command.GetCustomNamespaceResults(results)
                    .OnSuccessAsync(async customNamespaceResults =>
                    {
                        var typedMethodName = results.GetValue("ToTypedBuilderMethodName");
                        var builderConcreteName = customNamespaceResults.GetValue("BuilderConcreteName");
                        var generics = command.SourceModel.GetGenericTypeArgumentsString();
                        var builderName = results.GetValue(ResultNames.BuilderName).ToString().Replace(command.SourceModel.Name, builderConcreteName);
                        var builderConcreteTypeName = $"{customNamespaceResults.GetValue("CustomBuilderNamespace")}.{builderName}";
                        var builderTypeName = command.GetBuilderTypeName(customNamespaceResults.GetValue("CustomBuilderInterfaceNamespace"), customNamespaceResults.GetValue("CustomConcreteBuilderNamespace"), builderConcreteName, builderConcreteTypeName, results.GetValue(ResultNames.BuilderName));
                        var returnStatement = customNamespaceResults.GetValue("ReturnStatement");

                        response
                            .AddMethods(new MethodBuilder()
                                .WithName(methodName)
                                .WithAbstract(command.IsAbstract)
                                .WithOverride(command.Settings.BaseClass is not null)
                                .WithReturnTypeName(builderTypeName)
                                .AddReturnTypeGenericTypeArguments(command.Settings.BaseClass is not null
                                    ? Enumerable.Empty<ITypeContainerBuilder>()
                                    : command.SourceModel.GenericTypeArguments.Select(x => new PropertyBuilder().WithName("Dummy").WithTypeName(x)))
                                .AddCodeStatements(returnStatement));

                        if (command.Settings.EnableInheritance
                            && command.Settings.BaseClass is not null
                            && !string.IsNullOrEmpty(typedMethodName))
                        {
                            response
                                .AddMethods(new MethodBuilder()
                                    .WithName(typedMethodName)
                                    .WithReturnTypeName(builderConcreteTypeName)
                                    .AddReturnTypeGenericTypeArguments(command.SourceModel.GenericTypeArguments.Select(x => new PropertyBuilder().WithName("Dummy").WithTypeName(x)))
                                    .AddCodeStatements($"return new {builderConcreteTypeName}{generics}(this);"));
                        }
                        else if (command.Settings.UseCrossCuttingInterfaces)
                        {
                            var entityInterface = typeof(IBuildableEntity<object>).ReplaceGenericTypeName(builderTypeName);
                            if (!response.Interfaces.Contains(entityInterface))
                            {
                                response.AddInterfaces(entityInterface);
                            }
                        }

                        return await AddExplicitInterfaceImplementationsAsync(command, response, methodName, typedMethodName, token).ConfigureAwait(false);
                    }).ConfigureAwait(false);
            }).ConfigureAwait(false);
    }

    private async Task<Result> AddExplicitInterfaceImplementationsAsync(GenerateEntityCommand command, ClassBuilder response, string methodName, string typedMethodName, CancellationToken token)
    {
        if (!command.Settings.UseBuilderAbstractionsTypeConversion)
        {
            return Result.Success();
        }

        var interfaces = new List<Result<NameInfo>>();

        var sourceInterfaces = command.SourceModel.Interfaces
            .Where(
                x => (command.Settings.CopyInterfacePredicate?.Invoke(x) ?? true)
                  && command.Settings.BuilderAbstractionsTypeConversionNamespaces.Contains(x.GetNamespaceWithDefault()));

        foreach (var sourceInterface in sourceInterfaces)
        {
            var metadata = command.GetMappingMetadata(sourceInterface).ToArray();
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
                    command.FormatProvider,
                    new ParentChildContext<GenerateEntityCommand, Property>(command, property, command.Settings),
                    token
                ).ConfigureAwait(false)).Transform(y => new NameInfo(GetEntityName(command.Settings, sourceInterface, () => y.ToString()), y.ToString())));
            }
            else
            {
                interfaces.Add(Result.Success(new NameInfo(GetEntityName(command.Settings, sourceInterface, () => command.MapTypeName(sourceInterface.FixTypeName())), command.MapTypeName(sourceInterface.FixTypeName()))));
            }
        }

        var error = interfaces.Find(x => !x.IsSuccessful());
        if (error is not null)
        {
            return error;
        }

        var methodCallName = command.Settings.EnableInheritance
            && command.Settings.BaseClass is not null
            && !string.IsNullOrEmpty(typedMethodName)
                ? typedMethodName
                : methodName;

        response.AddMethods(interfaces.Select(x => new MethodBuilder()
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

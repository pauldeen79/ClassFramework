namespace ClassFramework.Pipelines.Entity.Components;

public class AddToBuilderMethodComponent(IFormattableStringParser formattableStringParser) : IPipelineComponent<EntityContext>
{
    private readonly IFormattableStringParser _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

    public Task<Result> ProcessAsync(PipelineContext<EntityContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        var results = new ResultDictionaryBuilder<GenericFormattableString>()
            .Add(NamedResults.Name, () => _formattableStringParser.Parse(context.Request.Settings.EntityNameFormatString, context.Request.FormatProvider, context.Request))
            .Add(NamedResults.Namespace, () => context.Request.GetMappingMetadata(context.Request.SourceModel.GetFullName()).GetGenericFormattableString(MetadataNames.CustomEntityNamespace, () => _formattableStringParser.Parse(context.Request.Settings.EntityNamespaceFormatString, context.Request.FormatProvider, context.Request)))
            .Add("ToBuilderMethodName", () => _formattableStringParser.Parse(context.Request.Settings.ToBuilderFormatString, context.Request.FormatProvider, context.Request))
            .Add("ToTypedBuilderMethodName", () => _formattableStringParser.Parse(context.Request.Settings.ToTypedBuilderFormatString, context.Request.FormatProvider, context.Request))
            .Add("BuilderName", () => _formattableStringParser.Parse(context.Request.Settings.BuilderNameFormatString, context.Request.FormatProvider, context.Request))
            .Build();

        var error = results.GetError();
        if (error is not null)
        {
            // Error in formattable string parsing
            return Task.FromResult<Result>(error);
        }

        var methodName = results["ToBuilderMethodName"].Value!;
        if (string.IsNullOrEmpty(methodName))
        {
            return Task.FromResult(Result.Success());
        }

        var typedMethodName = results["ToTypedBuilderMethodName"].Value!;

        var ns = results[NamedResults.Namespace].Value!.ToString();
        var name = results[NamedResults.Name].Value!.ToString();

        var entityFullName = $"{ns.AppendWhenNotNullOrEmpty(".")}{name}";
        if (context.Request.Settings.EnableInheritance && context.Request.Settings.BaseClass is not null)
        {
            entityFullName = entityFullName.ReplaceSuffix("Base", string.Empty, StringComparison.Ordinal);
        }

        var entityConcreteFullName = context.Request.Settings.EnableInheritance && context.Request.Settings.BaseClass is not null
            ? context.Request.Settings.BaseClass.GetFullName()
            : entityFullName;

        var metadata = context.Request.GetMappingMetadata(entityFullName).ToArray();
        var customNamespaceResults = new ResultDictionaryBuilder<string>()
            .Add("CustomBuilderNamespace", () => metadata.GetStringResult(MetadataNames.CustomBuilderNamespace, () => Result.Success($"{ns.AppendWhenNotNullOrEmpty(".")}Builders")))
            .Add("CustomBuilderInterfaceNamespace", () => metadata.GetStringResult(MetadataNames.CustomBuilderInterfaceNamespace, () => Result.Success($"{ns.AppendWhenNotNullOrEmpty(".")}Builders")))
            .Add("CustomConcreteBuilderNamespace", () => context.Request.GetMappingMetadata(entityConcreteFullName).GetStringResult(MetadataNames.CustomBuilderNamespace, () => Result.Success($"{ns.AppendWhenNotNullOrEmpty(".")}Builders")))
            .Build();
        var customNamespaceError = customNamespaceResults.GetError();
        if (customNamespaceError is not null)
        {
            // Error in formattable string parsing
            return Task.FromResult<Result>(customNamespaceError);
        }

        var builderConcreteName = context.Request.Settings.EnableInheritance && context.Request.Settings.BaseClass is null
            ? name
            : name.ReplaceSuffix("Base", string.Empty, StringComparison.Ordinal);

        var generics = context.Request.SourceModel.GetGenericTypeArgumentsString();
        var builderName = results["BuilderName"].Value!.ToString().Replace(context.Request.SourceModel.Name, builderConcreteName);
        var builderConcreteTypeName = $"{customNamespaceResults["CustomBuilderNamespace"].Value}.{builderName}";
        var builderTypeName = GetBuilderTypeName(context, customNamespaceResults["CustomBuilderInterfaceNamespace"], customNamespaceResults["CustomConcreteBuilderNamespace"], builderConcreteName, builderConcreteTypeName, results["BuilderName"]);

        var returnStatement = context.Request.Settings.EnableInheritance && context.Request.Settings.BaseClass is not null && !string.IsNullOrEmpty(typedMethodName)
            ? $"return {typedMethodName}();"
            : $"return new {builderConcreteTypeName}{generics}(this);";

        context.Request.Builder
            .AddMethods(new MethodBuilder()
                .WithName(methodName)
                .WithAbstract(context.Request.IsAbstract)
                .WithOverride(context.Request.Settings.BaseClass is not null)
                .WithReturnTypeName(builderTypeName)
                .AddReturnTypeGenericTypeArguments(context.Request.Settings.BaseClass is not null
                    ? Enumerable.Empty<ITypeContainerBuilder>()
                    : context.Request.SourceModel.GenericTypeArguments.Select(x => new PropertyBuilder().WithName("Dummy").WithTypeName(x)))
                .AddStringCodeStatements(returnStatement));

        if (context.Request.Settings.EnableInheritance
            && context.Request.Settings.BaseClass is not null
            && !string.IsNullOrEmpty(typedMethodName))
        {
            context.Request.Builder
                .AddMethods(new MethodBuilder()
                    .WithName(typedMethodName)
                    .WithReturnTypeName(builderConcreteTypeName)
                    .AddReturnTypeGenericTypeArguments(context.Request.SourceModel.GenericTypeArguments.Select(x => new PropertyBuilder().WithName("Dummy").WithTypeName(x)))
                    .AddStringCodeStatements($"return new {builderConcreteTypeName}{generics}(this);"));
        }

        return Task.FromResult(AddExplicitInterfaceImplementations(context, methodName, typedMethodName));
    }

    private static string GetBuilderTypeName(PipelineContext<EntityContext> context, Result<string> builderInterfaceNamespaceResult, Result<string> concreteBuilderNamespaceResult, string builderConcreteName, string builderConcreteTypeName, Result<GenericFormattableString> builderNameResult)
    {
        if (context.Request.Settings.InheritFromInterfaces)
        {
            if (context.Request.SourceModel.Interfaces.Count >= 2 && !context.Request.Settings.BuilderAbstractionsTypeConversionNamespaces.Contains(context.Request.SourceModel.Namespace))
            {
                var builderName = builderNameResult.Value!.ToString().Replace(context.Request.SourceModel.Name, context.Request.SourceModel.Interfaces.ElementAt(1).GetClassName());
                return $"{builderInterfaceNamespaceResult.Value}.{builderName}";
            }
            return $"{builderInterfaceNamespaceResult.Value}.I{builderConcreteName}Builder";
        }
        else if (context.Request.Settings.EnableInheritance && context.Request.Settings.BaseClass is not null)
        {
            var builderName = builderNameResult.Value!.ToString().Replace(context.Request.SourceModel.Name, context.Request.Settings.BaseClass.Name);
            return $"{concreteBuilderNamespaceResult.Value}.{builderName}";
        }
        else
        {
            return builderConcreteTypeName;
        }
    }

    private Result AddExplicitInterfaceImplementations(PipelineContext<EntityContext> context, string methodName, string typedMethodName)
    {
        if (!context.Request.Settings.UseBuilderAbstractionsTypeConversion)
        {
            return Result.Continue();
        }

        var interfaces = context.Request.SourceModel.Interfaces
            .Where(x => context.Request.Settings.CopyInterfacePredicate?.Invoke(x) ?? true)
            .Where(x => context.Request.Settings.BuilderAbstractionsTypeConversionNamespaces.Contains(x.GetNamespaceWithDefault()))
            .Select(x =>
            {
                var metadata = context.Request.GetMappingMetadata(x).ToArray();
                var ns = metadata.GetStringValue(MetadataNames.CustomBuilderInterfaceNamespace);

                if (!string.IsNullOrEmpty(ns))
                {
                    var property = new PropertyBuilder()
                        .WithName("Dummy")
                        .WithTypeName(x)
                        .Build();
                    var newTypeName = metadata.GetStringValue(MetadataNames.CustomBuilderInterfaceName, "I{NoGenerics(ClassName($property.TypeName))}Builder{GenericArguments($property.TypeName, true)}");
                    var newFullName = $"{ns}.{newTypeName}";

                    return _formattableStringParser.Parse
                    (
                        newFullName,
                        context.Request.FormatProvider,
                        new ParentChildContext<PipelineContext<EntityContext>, Property>(context, property, context.Request.Settings)
                    ).Transform(y => new { EntityName = x, BuilderName = y.ToString() });
                }
                return Result.Success(new { EntityName = x, BuilderName = context.Request.MapTypeName(x.FixTypeName()) });
            })
            .TakeWhileWithFirstNonMatching(x => x.IsSuccessful())
            .ToArray();

        var error = Array.Find(interfaces, x => !x.IsSuccessful());
        if (error is not null)
        {
            return error;
        }

        var methodCallName = context.Request.Settings.EnableInheritance && context.Request.Settings.BaseClass is not null && !string.IsNullOrEmpty(typedMethodName)
            ? typedMethodName
            : methodName;

        context.Request.Builder.AddMethods(interfaces.Select(x => new MethodBuilder()
            .WithName(methodName)
            .WithReturnTypeName(x.Value!.BuilderName)
            .WithExplicitInterfaceName(x.Value!.EntityName)
            .AddStringCodeStatements($"return {methodCallName}();")));

        return Result.Success();
    }
}

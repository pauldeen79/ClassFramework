namespace ClassFramework.Pipelines.Entity.Components;

public class AddToBuilderMethodComponentBuilder(IFormattableStringParser formattableStringParser) : IEntityComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

    public IPipelineComponent<EntityContext> Build()
        => new AddToBuilderMethodComponent(_formattableStringParser);
}

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

        var metadata = context.Request.GetMappingMetadata(entityFullName);
        var builderNamespaceResult = metadata.GetStringResult(MetadataNames.CustomBuilderNamespace, () => Result.Success($"{ns.AppendWhenNotNullOrEmpty(".")}Builders"));
        var builderInterfaceNamespaceResult = metadata.GetStringResult(MetadataNames.CustomBuilderInterfaceNamespace, () => Result.Success($"{ns.AppendWhenNotNullOrEmpty(".")}Builders"));
        var concreteBuilderNamespaceResult = context.Request.GetMappingMetadata(entityConcreteFullName).GetStringResult(MetadataNames.CustomBuilderNamespace, () => Result.Success($"{ns.AppendWhenNotNullOrEmpty(".")}Builders"));

        var builderConcreteName = context.Request.Settings.EnableInheritance && context.Request.Settings.BaseClass is null
            ? name
            : name.ReplaceSuffix("Base", string.Empty, StringComparison.Ordinal);

        var generics = context.Request.SourceModel.GetGenericTypeArgumentsString();
        var builderConcreteTypeName = $"{builderNamespaceResult.Value}.{builderConcreteName}Builder{generics}";
        var builderTypeName = GetBuilderTypeName(context, builderInterfaceNamespaceResult, concreteBuilderNamespaceResult, builderConcreteName, builderConcreteTypeName);

        var returnStatement = context.Request.Settings.EnableInheritance && context.Request.Settings.BaseClass is not null && !string.IsNullOrEmpty(typedMethodName)
            ? $"return {typedMethodName}();"
            : $"return new {builderConcreteTypeName}(this);";

        context.Request.Builder
            .AddMethods(new MethodBuilder()
                .WithName(methodName)
                .WithAbstract(context.Request.IsAbstract)
                .WithOverride(context.Request.Settings.BaseClass is not null)
                .WithReturnTypeName(builderTypeName)
                .AddStringCodeStatements(returnStatement));

        if (context.Request.Settings.EnableInheritance
            && context.Request.Settings.BaseClass is not null
            && !string.IsNullOrEmpty(typedMethodName))
        {
            context.Request.Builder
                .AddMethods(new MethodBuilder()
                    .WithName(typedMethodName)
                    .WithReturnTypeName(builderConcreteTypeName)
                    .AddStringCodeStatements($"return new {builderConcreteTypeName}(this);"));
        }

        return Task.FromResult(Result.Success());
    }

    private static string GetBuilderTypeName(PipelineContext<EntityContext> context, Result<string> builderInterfaceNamespaceResult, Result<string> concreteBuilderNamespaceResult, string builderConcreteName, string builderConcreteTypeName)
    {
        if (context.Request.Settings.InheritFromInterfaces)
        {
            if (context.Request.SourceModel.Interfaces.Count >= 2)
            {
                return $"{builderInterfaceNamespaceResult.Value}.{context.Request.SourceModel.Interfaces.ElementAt(1).GetClassName()}Builder";
            }
            return $"{builderInterfaceNamespaceResult.Value}.I{builderConcreteName}Builder";
        }
        else if (context.Request.Settings.EnableInheritance && context.Request.Settings.BaseClass is not null)
        {
            return $"{concreteBuilderNamespaceResult.Value}.{context.Request.Settings.BaseClass.Name}Builder";
        }
        else
        {
            return builderConcreteTypeName;
        }
    }
}

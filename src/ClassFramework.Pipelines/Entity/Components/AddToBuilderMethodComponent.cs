namespace ClassFramework.Pipelines.Entity.Features;

public class AddToBuilderMethodComponentBuilder : IEntityComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddToBuilderMethodComponentBuilder(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public IPipelineComponent<IConcreteTypeBuilder, EntityContext> Build()
        => new AddToBuilderMethodComponent(_formattableStringParser);
}

public class AddToBuilderMethodComponent : IPipelineComponent<IConcreteTypeBuilder, EntityContext>
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddToBuilderMethodComponent(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public Task<Result<IConcreteTypeBuilder>> Process(PipelineContext<IConcreteTypeBuilder, EntityContext> context)
    {
        context = context.IsNotNull(nameof(context));

        var resultSetBuilder = new NamedResultSetBuilder<string>();
        resultSetBuilder.Add(NamedResults.Name, () => _formattableStringParser.Parse(context.Context.Settings.EntityNameFormatString, context.Context.FormatProvider, context));
        resultSetBuilder.Add(NamedResults.Namespace, () => context.Context.GetMappingMetadata(context.Context.SourceModel.GetFullName()).GetStringResult(MetadataNames.CustomEntityNamespace, () => _formattableStringParser.Parse(context.Context.Settings.EntityNamespaceFormatString, context.Context.FormatProvider, context)));
        resultSetBuilder.Add("ToBuilderMethodName", () => _formattableStringParser.Parse(context.Context.Settings.ToBuilderFormatString, context.Context.FormatProvider, context));
        resultSetBuilder.Add("ToTypedBuilderMethodName", () => _formattableStringParser.Parse(context.Context.Settings.ToTypedBuilderFormatString, context.Context.FormatProvider, context));
        var results = resultSetBuilder.Build();

        var error = Array.Find(results, x => !x.Result.IsSuccessful());
        if (error is not null)
        {
            // Error in formattable string parsing
            return Result.FromExistingResult<IConcreteTypeBuilder>(error.Result);
        }

        var methodName = results.First(x => x.Name == "ToBuilderMethodName").Result.Value!;
        if (string.IsNullOrEmpty(methodName))
        {
            return Result.Continue<IConcreteTypeBuilder>();
        }

        var typedMethodName = results.First(x => x.Name == "ToTypedBuilderMethodName").Result.Value!;

        var ns = results.First(x => x.Name == NamedResults.Namespace).Result.Value!;
        var name = results.First(x => x.Name == NamedResults.Name).Result.Value!;

        var entityFullName = $"{ns.AppendWhenNotNullOrEmpty(".")}{name}";
        if (context.Context.Settings.EnableInheritance && context.Context.Settings.BaseClass is not null)
        {
            entityFullName = entityFullName.ReplaceSuffix("Base", string.Empty, StringComparison.Ordinal);
        }

        var entityConcreteFullName = context.Context.Settings.EnableInheritance && context.Context.Settings.BaseClass is not null
            ? context.Context.Settings.BaseClass.GetFullName()
            : entityFullName;

        var metadata = context.Context.GetMappingMetadata(entityFullName);
        var builderNamespaceResult = metadata.GetStringResult(MetadataNames.CustomBuilderNamespace, () => Result.Success($"{ns.AppendWhenNotNullOrEmpty(".")}Builders"));
        var builderInterfaceNamespaceResult = metadata.GetStringResult(MetadataNames.CustomBuilderInterfaceNamespace, () => Result.Success($"{ns.AppendWhenNotNullOrEmpty(".")}Builders"));
        var concreteBuilderNamespaceResult = context.Context.GetMappingMetadata(entityConcreteFullName).GetStringResult(MetadataNames.CustomBuilderNamespace, () => Result.Success($"{ns.AppendWhenNotNullOrEmpty(".")}Builders"));

        var builderConcreteName = context.Context.Settings.EnableInheritance && context.Context.Settings.BaseClass is null
            ? name
            : name.ReplaceSuffix("Base", string.Empty, StringComparison.Ordinal);

        var generics = context.Context.SourceModel.GetGenericTypeArgumentsString();
        var builderConcreteTypeName = $"{builderNamespaceResult.Value}.{builderConcreteName}Builder{generics}";
        var builderTypeName = GetBuilderTypeName(context, builderInterfaceNamespaceResult, concreteBuilderNamespaceResult, builderConcreteName, builderConcreteTypeName);

        var returnStatement = context.Context.Settings.EnableInheritance && context.Context.Settings.BaseClass is not null && !string.IsNullOrEmpty(typedMethodName)
            ? $"return {typedMethodName}();"
            : $"return new {builderConcreteTypeName}(this);";

        context.Model
            .AddMethods(new MethodBuilder()
                .WithName(methodName)
                .WithAbstract(context.Context.IsAbstract)
                .WithOverride(context.Context.Settings.BaseClass is not null)
                .WithReturnTypeName(builderTypeName)
                .AddStringCodeStatements(returnStatement));

        if (context.Context.Settings.EnableInheritance
            && context.Context.Settings.BaseClass is not null
            && !string.IsNullOrEmpty(typedMethodName))
        {
            context.Model
                .AddMethods(new MethodBuilder()
                    .WithName(typedMethodName)
                    .WithReturnTypeName(builderConcreteTypeName)
                    .AddStringCodeStatements($"return new {builderConcreteTypeName}(this);"));
        }

        return Result.Continue<IConcreteTypeBuilder>();
    }

    private static string GetBuilderTypeName(PipelineContext<IConcreteTypeBuilder, EntityContext> context, Result<string> builderInterfaceNamespaceResult, Result<string> concreteBuilderNamespaceResult, string builderConcreteName, string builderConcreteTypeName)
    {
        if (context.Context.Settings.InheritFromInterfaces)
        {
            if (context.Context.SourceModel.Interfaces.Count >= 2)
            {
                return $"{builderInterfaceNamespaceResult.Value}.{context.Context.SourceModel.Interfaces.ElementAt(1).GetClassName()}Builder";
            }
            return $"{builderInterfaceNamespaceResult.Value}.I{builderConcreteName}Builder";
        }
        else if (context.Context.Settings.EnableInheritance && context.Context.Settings.BaseClass is not null)
        {
            return $"{concreteBuilderNamespaceResult.Value}.{context.Context.Settings.BaseClass.Name}Builder";
        }
        else 
        {
            return builderConcreteTypeName;
        }
    }
}

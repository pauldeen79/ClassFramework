﻿namespace ClassFramework.Pipelines.Entity.Components;

public class AddToBuilderMethodComponent(IExpressionEvaluator evaluator) : IPipelineComponent<EntityContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ProcessAsync(PipelineContext<EntityContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        var results = await new AsyncResultDictionaryBuilder<GenericFormattableString>()
            .Add(NamedResults.Name, _evaluator.EvaluateInterpolatedStringAsync(context.Request.Settings.EntityNameFormatString, context.Request.FormatProvider, context.Request, token))
            .Add(NamedResults.Namespace, context.Request.GetMappingMetadata(context.Request.SourceModel.GetFullName()).GetGenericFormattableStringAsync(MetadataNames.CustomEntityNamespace, _evaluator.EvaluateInterpolatedStringAsync(context.Request.Settings.EntityNamespaceFormatString, context.Request.FormatProvider, context.Request, token)))
            .Add("BuilderInterfaceNamespace", context.Request.GetMappingMetadata(context.Request.SourceModel.GetFullName()).GetGenericFormattableStringAsync(MetadataNames.CustomBuilderInterfaceNamespace, _evaluator.EvaluateInterpolatedStringAsync(context.Request.Settings.BuilderNamespaceFormatString, context.Request.FormatProvider, context.Request, token)))
            .Add("ToBuilderMethodName", _evaluator.EvaluateInterpolatedStringAsync(context.Request.Settings.ToBuilderFormatString, context.Request.FormatProvider, context.Request, token))
            .Add("ToTypedBuilderMethodName", _evaluator.EvaluateInterpolatedStringAsync(context.Request.Settings.ToTypedBuilderFormatString, context.Request.FormatProvider, context.Request, token))
            .Add("BuilderName", _evaluator.EvaluateInterpolatedStringAsync(context.Request.Settings.BuilderNameFormatString, context.Request.FormatProvider, context.Request, token))
            .Build()
            .ConfigureAwait(false);

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

        var ns = results.GetValue(NamedResults.Namespace).ToString();
        var name = results.GetValue(NamedResults.Name).ToString();

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
            .Add("CustomBuilderInterfaceNamespace", () => metadata.GetStringResult(MetadataNames.CustomBuilderInterfaceNamespace, () => Result.Success(GetBuilderInterfaceNamespace(context, results, ns))))
            .Add("CustomConcreteBuilderNamespace", () => context.Request.GetMappingMetadata(entityConcreteFullName).GetStringResult(MetadataNames.CustomBuilderNamespace, () => Result.Success($"{ns.AppendWhenNotNullOrEmpty(".")}Builders")))
            .Build();
        var customNamespaceError = customNamespaceResults.GetError();
        if (customNamespaceError is not null)
        {
            // Error in formattable string parsing
            return customNamespaceError;
        }

        var builderConcreteName = context.Request.Settings.EnableInheritance && context.Request.Settings.BaseClass is null
            ? name
            : name.ReplaceSuffix("Base", string.Empty, StringComparison.Ordinal);

        var generics = context.Request.SourceModel.GetGenericTypeArgumentsString();
        var builderName = results.GetValue("BuilderName").ToString().Replace(context.Request.SourceModel.Name, builderConcreteName);
        var builderConcreteTypeName = $"{customNamespaceResults.GetValue("CustomBuilderNamespace")}.{builderName}";
        var builderTypeName = GetBuilderTypeName(context, customNamespaceResults.GetValue("CustomBuilderInterfaceNamespace"), customNamespaceResults.GetValue("CustomConcreteBuilderNamespace"), builderConcreteName, builderConcreteTypeName, results.GetValue("BuilderName"));

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

        return await AddExplicitInterfaceImplementations(context, methodName, typedMethodName, token).ConfigureAwait(false);
    }

    private static string GetBuilderInterfaceNamespace(PipelineContext<EntityContext> context, IReadOnlyDictionary<string, Result<GenericFormattableString>> results, string ns)
        => context.Request.Settings.InheritFromInterfaces
            ? results.GetValue("BuilderInterfaceNamespace").ToString()
            : $"{ns.AppendWhenNotNullOrEmpty(".")}Builders";

    private static string GetBuilderTypeName(PipelineContext<EntityContext> context, string builderInterfaceNamespace, string concreteBuilderNamespace, string builderConcreteName, string builderConcreteTypeName, string builderNameValue)
    {
        if (context.Request.Settings.InheritFromInterfaces)
        {
            if (context.Request.SourceModel.Interfaces.Count >= 2 && !context.Request.Settings.BuilderAbstractionsTypeConversionNamespaces.Contains(context.Request.SourceModel.Namespace))
            {
                var builderName = builderNameValue.Replace(context.Request.SourceModel.Name, context.Request.SourceModel.Interfaces.ElementAt(1).GetClassName());
                return $"{builderInterfaceNamespace}.{builderName}";
            }
            return $"{builderInterfaceNamespace}.I{builderConcreteName}Builder";
        }
        else if (context.Request.Settings.EnableInheritance && context.Request.Settings.BaseClass is not null)
        {
            var builderName = builderNameValue.Replace(context.Request.SourceModel.Name, context.Request.Settings.BaseClass.Name);
            return $"{concreteBuilderNamespace}.{builderName}";
        }
        else
        {
            return builderConcreteTypeName;
        }
    }

    private async Task<Result> AddExplicitInterfaceImplementations(PipelineContext<EntityContext> context, string methodName, string typedMethodName, CancellationToken token)
    {
        if (!context.Request.Settings.UseBuilderAbstractionsTypeConversion)
        {
            return Result.Continue();
        }

        var interfaces = new List<Result<NameInfo>>();

        foreach (var x in context.Request.SourceModel.Interfaces
            .Where(x => context.Request.Settings.CopyInterfacePredicate?.Invoke(x) ?? true)
            .Where(x => context.Request.Settings.BuilderAbstractionsTypeConversionNamespaces.Contains(x.GetNamespaceWithDefault())))
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
                ).ConfigureAwait(false)).Transform(y => new NameInfo { EntityName = x, BuilderName = y.ToString() }));
            }
            else
            {
                interfaces.Add(Result.Success(new NameInfo { EntityName = x, BuilderName = context.Request.MapTypeName(x.FixTypeName()) }));
            }

        }

        var error = interfaces.Find(x => !x.IsSuccessful());
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

    private sealed class NameInfo
    {
        public string EntityName { get; set; } = default!;
        public string BuilderName { get; set; } = default!;
    }
}

namespace ClassFramework.Pipelines.Entity;

public class EntityContext(TypeBase sourceModel, PipelineSettings settings, IFormatProvider formatProvider, CancellationToken cancellationToken) : ContextBase<TypeBase>(sourceModel, settings, formatProvider, cancellationToken)
{
    public IEnumerable<Property> GetSourceProperties()
        => SourceModel.Properties.Where(x => SourceModel.IsMemberValidForBuilderClass(x, Settings));

    public bool IsAbstract
        => Settings.EnableInheritance
        && Settings.IsAbstract;

    protected override string NewCollectionTypeName => Settings.EntityNewCollectionTypeName;

    public ClassBuilder Builder { get; } = new();

    public string GetBuilderTypeName(
        string builderInterfaceNamespace,
        string concreteBuilderNamespace,
        string builderConcreteName,
        string builderConcreteTypeName,
        string builderNameValue)
    {
        builderInterfaceNamespace = ArgumentGuard.IsNotNull(builderInterfaceNamespace, nameof(builderInterfaceNamespace));
        concreteBuilderNamespace = ArgumentGuard.IsNotNull(concreteBuilderNamespace, nameof(concreteBuilderNamespace));
        builderConcreteName = ArgumentGuard.IsNotNull(builderConcreteName, nameof(builderConcreteName));
        builderConcreteTypeName = ArgumentGuard.IsNotNull(builderConcreteTypeName, nameof(builderConcreteTypeName));
        builderNameValue = ArgumentGuard.IsNotNull(builderNameValue, nameof(builderNameValue));

        if (Settings.InheritFromInterfaces)
        {
            if (SourceModel.Interfaces.Count >= 2 && !Settings.BuilderAbstractionsTypeConversionNamespaces.Contains(SourceModel.Namespace))
            {
                var builderName = builderNameValue.Replace(SourceModel.Name, SourceModel.Interfaces.ElementAt(1).GetClassName());
                return $"{builderInterfaceNamespace}.{builderName}";
            }
            return $"{builderInterfaceNamespace}.I{builderConcreteName}Builder";
        }
        else if (Settings.EnableInheritance && Settings.BaseClass is not null)
        {
            var builderName = builderNameValue.Replace(SourceModel.Name, Settings.BaseClass.Name);
            return $"{concreteBuilderNamespace}.{builderName}";
        }
        else
        {
            return builderConcreteTypeName;
        }
    }

    public string GetEntityFullName(string @namespace, string name)
    {
        ArgumentGuard.IsNotNull(@namespace, nameof(@namespace));
        ArgumentGuard.IsNotNull(name, nameof(name));

        var entityFullName = $"{@namespace.AppendWhenNotNullOrEmpty(".")}{name}";
        if (Settings.EnableInheritance && Settings.BaseClass is not null)
        {
            entityFullName = entityFullName.ReplaceSuffix("Base", string.Empty, StringComparison.Ordinal);
        }

        return entityFullName;
    }

    public string GetEntityConcreteFullName(string @namespace, string name)
    {
        ArgumentGuard.IsNotNull(@namespace, nameof(@namespace));
        ArgumentGuard.IsNotNull(name, nameof(name));

        return Settings.EnableInheritance && Settings.BaseClass is not null
            ? Settings.BaseClass.GetFullName()
            : GetEntityFullName(@namespace, name);
    }

    public string GetBuilderInterfaceNamespace(string builderInterfaceNamespace, string @namespace)
    {
        ArgumentGuard.IsNotNull(builderInterfaceNamespace, nameof(builderInterfaceNamespace));
        ArgumentGuard.IsNotNull(@namespace, nameof(@namespace));

        return Settings.InheritFromInterfaces
            ? builderInterfaceNamespace
            : $"{@namespace.AppendWhenNotNullOrEmpty(".")}Builders";
    }

    public async Task<IReadOnlyDictionary<string, Result<GenericFormattableString>>> GetToBuilderResultsAsync(IExpressionEvaluator evaluator, CancellationToken token)
    {
        ArgumentGuard.IsNotNull(evaluator, nameof(evaluator));

        return await new AsyncResultDictionaryBuilder<GenericFormattableString>()
            .Add(ResultNames.Name, () => evaluator.EvaluateInterpolatedStringAsync(Settings.EntityNameFormatString, FormatProvider, this, token))
            .Add(ResultNames.Namespace, () => GetMappingMetadata(SourceModel.GetFullName()).GetGenericFormattableStringAsync(MetadataNames.CustomEntityNamespace, evaluator.EvaluateInterpolatedStringAsync(Settings.EntityNamespaceFormatString, FormatProvider, this, token)))
            .Add("BuilderInterfaceNamespace", () => GetMappingMetadata(SourceModel.GetFullName()).GetGenericFormattableStringAsync(MetadataNames.CustomBuilderInterfaceNamespace, evaluator.EvaluateInterpolatedStringAsync(Settings.BuilderNamespaceFormatString, FormatProvider, this, token)))
            .Add("ToBuilderMethodName", () => evaluator.EvaluateInterpolatedStringAsync(Settings.ToBuilderFormatString, FormatProvider, this, token))
            .Add("ToTypedBuilderMethodName", () => evaluator.EvaluateInterpolatedStringAsync(Settings.ToTypedBuilderFormatString, FormatProvider, this, token))
            .Add(ResultNames.BuilderName, () => evaluator.EvaluateInterpolatedStringAsync(Settings.BuilderNameFormatString, FormatProvider, this, token))
            .Build()
            .ConfigureAwait(false);
    }

    public IReadOnlyDictionary<string, Result<string>> GetCustomNamespaceResults(IReadOnlyDictionary<string, Result<GenericFormattableString>> results)
    {
        ArgumentGuard.IsNotNull(results, nameof(results));

        var name = results.GetValue(ResultNames.Name).ToString();
        var ns = results.GetValue(ResultNames.Namespace).ToString();
        var entityConcreteFullName = GetEntityConcreteFullName(ns, name);
        var entityFullName = GetEntityFullName(ns, name);
        var typedMethodName = results.GetValue("ToTypedBuilderMethodName");
        var metadata = GetMappingMetadata(entityFullName).ToArray();

        var resultDictionary = new ResultDictionaryBuilder<string>()
            .Add("CustomBuilderNamespace", () => metadata.GetStringResult(MetadataNames.CustomBuilderNamespace, () => Result.Success($"{ns.AppendWhenNotNullOrEmpty(".")}Builders")))
            .Add("CustomBuilderInterfaceNamespace", () => metadata.GetStringResult(MetadataNames.CustomBuilderInterfaceNamespace, () => Result.Success(GetBuilderInterfaceNamespace(results.GetValue("BuilderInterfaceNamespace").ToString(), ns))))
            .Add("CustomConcreteBuilderNamespace", () => GetMappingMetadata(entityConcreteFullName).GetStringResult(MetadataNames.CustomBuilderNamespace, () => Result.Success($"{ns.AppendWhenNotNullOrEmpty(".")}Builders")))
            .Add("BuilderConcreteName", () => Result.Success(Settings.EnableInheritance && Settings.BaseClass is null
                ? name
                : name.ReplaceSuffix("Base", string.Empty, StringComparison.Ordinal)))
            .Build()
            .ToDictionary(x => x.Key, x => x.Value);

        if (resultDictionary.GetError() is null)
        {
            resultDictionary.Add("ReturnStatement", Result.Success(Settings.EnableInheritance
                    && Settings.BaseClass is not null
                    && !string.IsNullOrEmpty(typedMethodName)
                        ? $"return {typedMethodName}();"
                        : $"return new {resultDictionary.GetValue("CustomBuilderNamespace")}.{results.GetValue(ResultNames.BuilderName).ToString().Replace(SourceModel.Name, resultDictionary.GetValue("BuilderConcreteName"))}{SourceModel.GetGenericTypeArgumentsString()}(this);"));
        }

        return resultDictionary;
    }
}

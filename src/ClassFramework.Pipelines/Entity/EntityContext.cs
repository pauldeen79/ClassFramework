namespace ClassFramework.Pipelines.Entity;

public class EntityContext(TypeBase sourceModel, PipelineSettings settings, IFormatProvider formatProvider, CancellationToken cancellationToken) : ContextBase<TypeBase>(sourceModel, settings, formatProvider, cancellationToken)
{
    public IEnumerable<Property> GetSourceProperties()
        => SourceModel.Properties.Where(x => SourceModel.IsMemberValidForBuilderClass(x, Settings));

    public bool IsAbstract
        => Settings.EnableInheritance
        && Settings.IsAbstract;

    protected override string NewCollectionTypeName => Settings.EntityNewCollectionTypeName;

    public ClassBuilder Builder => _wrappedBuilder.Builder;

    private readonly ClassBuilderWrapper _wrappedBuilder = new();

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
        var entityFullName = $"{@namespace.AppendWhenNotNullOrEmpty(".")}{name}";
        if (Settings.EnableInheritance && Settings.BaseClass is not null)
        {
            entityFullName = entityFullName.ReplaceSuffix("Base", string.Empty, StringComparison.Ordinal);
        }

        return entityFullName;
    }

    public string GetEntityConcreteFullName(string @namespace, string name)
        => Settings.EnableInheritance && Settings.BaseClass is not null
            ? Settings.BaseClass.GetFullName()
            : GetEntityFullName(@namespace, name);

    public string GetBuilderInterfaceNamespace(string builderInterfaceNamespace, string @namespace)
        => Settings.InheritFromInterfaces
            ? builderInterfaceNamespace
            : $"{@namespace.AppendWhenNotNullOrEmpty(".")}Builders";

    public async Task<IReadOnlyDictionary<string, Result<GenericFormattableString>>> GetToBuilderResultsAsync(IExpressionEvaluator evaluator, CancellationToken token)
        => await new AsyncResultDictionaryBuilder<GenericFormattableString>()
            .Add(ResultNames.Name, evaluator.EvaluateInterpolatedStringAsync(Settings.EntityNameFormatString, FormatProvider, this, token))
            .Add(ResultNames.Namespace, GetMappingMetadata(SourceModel.GetFullName()).GetGenericFormattableStringAsync(MetadataNames.CustomEntityNamespace, evaluator.EvaluateInterpolatedStringAsync(Settings.EntityNamespaceFormatString, FormatProvider, this, token)))
            .Add("BuilderInterfaceNamespace", GetMappingMetadata(SourceModel.GetFullName()).GetGenericFormattableStringAsync(MetadataNames.CustomBuilderInterfaceNamespace, evaluator.EvaluateInterpolatedStringAsync(Settings.BuilderNamespaceFormatString, FormatProvider, this, token)))
            .Add("ToBuilderMethodName", evaluator.EvaluateInterpolatedStringAsync(Settings.ToBuilderFormatString, FormatProvider, this, token))
            .Add("ToTypedBuilderMethodName", evaluator.EvaluateInterpolatedStringAsync(Settings.ToTypedBuilderFormatString, FormatProvider, this, token))
            .Add(ResultNames.BuilderName, evaluator.EvaluateInterpolatedStringAsync(Settings.BuilderNameFormatString, FormatProvider, this, token))
            .Build()
            .ConfigureAwait(false);
}

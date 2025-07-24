namespace ClassFramework.Pipelines.BuilderExtension;

public class BuilderExtensionContext(TypeBase sourceModel, PipelineSettings settings, IFormatProvider formatProvider, CancellationToken cancellationToken) : ContextBase<TypeBase>(sourceModel, settings, formatProvider, cancellationToken)
{
    protected override string NewCollectionTypeName => Settings.BuilderNewCollectionTypeName;

    public IEnumerable<Property> GetSourceProperties()
        => SourceModel.Properties.Where(x => SourceModel.IsMemberValidForBuilderClass(x, Settings));

    public ClassBuilder Builder => _wrappedBuilder.Builder;

    public string GetReturnTypeForFluentMethod(string builderNamespace, string builderName)
        => $"{builderNamespace.AppendWhenNotNullOrEmpty(".")}{builderName}{SourceModel.GetGenericTypeArgumentsString()}";

    private readonly ClassBuilderWrapper _wrappedBuilder = new();
}

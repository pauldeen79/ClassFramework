namespace ClassFramework.Pipelines.BuilderExtension;

public class BuilderExtensionContext(TypeBase sourceModel, PipelineSettings settings, IFormatProvider formatProvider) : ContextBase<TypeBase>(sourceModel, settings, formatProvider)

{
    protected override string NewCollectionTypeName => Settings.BuilderNewCollectionTypeName;

    public IEnumerable<Property> GetSourceProperties()
        => SourceModel.Properties.Where(x => SourceModel.IsMemberValidForBuilderClass(x, Settings));

    public ClassBuilder Builder => _wrappedBuilder.Builder;

    private readonly ClassBuilderWrapper _wrappedBuilder = new();
}

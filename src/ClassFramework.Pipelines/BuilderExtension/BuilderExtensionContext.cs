namespace ClassFramework.Pipelines.BuilderExtension;

public class BuilderExtensionContext : ContextBase<IType>
{
    public BuilderExtensionContext(IType sourceModel, PipelineSettings settings, IFormatProvider formatProvider)
        : base(sourceModel, settings, formatProvider)
    {
    }

    protected override string NewCollectionTypeName => Settings.BuilderNewCollectionTypeName;

    public override object CreateModel() => new ClassBuilder();

    public IEnumerable<Property> GetSourceProperties()
        => SourceModel.Properties.Where(x => SourceModel.IsMemberValidForBuilderClass(x, Settings));
}

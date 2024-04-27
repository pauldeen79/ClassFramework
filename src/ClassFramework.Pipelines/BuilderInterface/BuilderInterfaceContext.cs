namespace ClassFramework.Pipelines.BuilderInterface;

public class BuilderInterfaceContext : ContextBase<IType>
{
    public BuilderInterfaceContext(IType sourceModel, PipelineSettings settings, IFormatProvider formatProvider)
        : base(sourceModel, settings, formatProvider)
    {
    }

    public override object CreateModel() => new InterfaceBuilder();

    protected override string NewCollectionTypeName => Settings.EntityNewCollectionTypeName;
}

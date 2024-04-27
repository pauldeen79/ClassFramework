namespace ClassFramework.Pipelines.Interface;

public class InterfaceContext : ContextBase<IType>
{
    public InterfaceContext(IType sourceModel, PipelineSettings settings, IFormatProvider formatProvider)
        : base(sourceModel, settings, formatProvider)
    {
    }

    public override object CreateModel() => new InterfaceBuilder();

    protected override string NewCollectionTypeName => Settings.EntityNewCollectionTypeName;
}

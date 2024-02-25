namespace ClassFramework.Pipelines.OverrideEntity;

public class OverrideEntityContext : ContextBase<IType>
{
    public OverrideEntityContext(IType sourceModel, PipelineSettings settings, IFormatProvider formatProvider)
        : base(sourceModel, settings, formatProvider)
    {
    }

    public bool IsAbstract
        => Settings.EnableInheritance
        && Settings.IsAbstract;

    protected override string NewCollectionTypeName => Settings.EntityNewCollectionTypeName;
}

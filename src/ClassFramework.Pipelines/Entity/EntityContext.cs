namespace ClassFramework.Pipelines.Entity;

public class EntityContext : ContextBase<IType>
{
    public EntityContext(IType sourceModel, PipelineSettings settings, IFormatProvider formatProvider)
        : base(sourceModel, settings, formatProvider)
    {
    }

    public bool IsAbstract
        => Settings.EnableInheritance
        && Settings.IsAbstract;

    public override object CreateModel() => new ClassBuilder();

    protected override string NewCollectionTypeName => Settings.EntityNewCollectionTypeName;
}

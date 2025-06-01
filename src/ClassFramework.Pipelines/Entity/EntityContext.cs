namespace ClassFramework.Pipelines.Entity;

public class EntityContext(TypeBase sourceModel, PipelineSettings settings, IFormatProvider formatProvider, CancellationToken cancellationToken) : ContextBase<TypeBase>(sourceModel, settings, formatProvider, cancellationToken)
{
    public bool IsAbstract
        => Settings.EnableInheritance
        && Settings.IsAbstract;

    protected override string NewCollectionTypeName => Settings.EntityNewCollectionTypeName;

    public ClassBuilder Builder => _wrappedBuilder.Builder;

    private readonly ClassBuilderWrapper _wrappedBuilder = new();
}

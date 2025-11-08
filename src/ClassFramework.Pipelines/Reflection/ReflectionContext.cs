namespace ClassFramework.Pipelines.Reflection;

public class ReflectionContext : ContextBase<Type>
{
    public ReflectionContext(Type sourceModel, PipelineSettings settings, IFormatProvider formatProvider, CancellationToken cancellationToken)
        : base(sourceModel, settings, formatProvider, cancellationToken)
    {
    }

    protected override string NewCollectionTypeName => Settings.EntityNewCollectionTypeName;

    public override bool SourceModelHasNoProperties() => SourceModel.GetProperties().Length == 0;
}

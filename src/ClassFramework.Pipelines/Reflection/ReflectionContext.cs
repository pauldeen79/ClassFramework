namespace ClassFramework.Pipelines.Reflection;

public class ReflectionContext : ContextBase<Type>
{
    public ReflectionContext(Type sourceModel, PipelineSettings settings, IFormatProvider formatProvider, CancellationToken cancellationToken)
        : base(sourceModel, settings, formatProvider, cancellationToken)
    {
        _wrappedBuilder = new TypeBaseBuilderWrapper(SourceModel);
    }

    protected override string NewCollectionTypeName => Settings.EntityNewCollectionTypeName;

    public TypeBaseBuilder Builder => _wrappedBuilder.Builder;

    private readonly TypeBaseBuilderWrapper _wrappedBuilder;

    public override object GetResponse() => Builder;
}

namespace ClassFramework.Pipelines.Reflection;

public class ReflectionContext : ContextBase<Type>
{
    private readonly TypeBaseBuilderWrapper _wrappedBuilder;

    public ReflectionContext(Type sourceModel, PipelineSettings settings, IFormatProvider formatProvider, CancellationToken cancellationToken)
        : base(sourceModel, settings, formatProvider, cancellationToken)
    {
        _wrappedBuilder = new TypeBaseBuilderWrapper(SourceModel);
    }

    protected override string NewCollectionTypeName => Settings.EntityNewCollectionTypeName;

    public TypeBaseBuilder Builder => _wrappedBuilder.Builder;

    public override object GetResponse() => Builder;

    public override bool HasNoProperties() => SourceModel.GetProperties().Length == 0;
}

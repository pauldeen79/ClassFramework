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

    public override object GetResponseBuilder() => Builder;

    public override object GetResponseEntity() => Builder.Build();

    public override bool SourceModelHasNoProperties() => SourceModel.GetProperties().Length == 0;
}

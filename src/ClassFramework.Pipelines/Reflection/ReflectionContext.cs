namespace ClassFramework.Pipelines.Reflection;

public class ReflectionContext : ContextBase<Type, TypeBase>
{
    public ReflectionContext(Type sourceModel, PipelineSettings settings, IFormatProvider formatProvider)
        : base(sourceModel, settings, formatProvider)
    {
        _wrappedBuilder = new TypeBaseBuilderWrapper(SourceModel);
    }

    protected override string NewCollectionTypeName => Settings.EntityNewCollectionTypeName;

    protected override IBuilder<TypeBase> CreateResponseBuilder() => _wrappedBuilder;

    public TypeBaseBuilder Builder => _wrappedBuilder.Builder;

    private readonly TypeBaseBuilderWrapper _wrappedBuilder;
}

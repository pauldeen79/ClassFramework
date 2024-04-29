namespace ClassFramework.Pipelines.Reflection;

public class ReflectionContext : ContextBase<Type, TypeBase>
{
    public ReflectionContext(Type sourceModel, PipelineSettings settings, IFormatProvider formatProvider)
        : base(sourceModel, settings, formatProvider)
    {
    }

    protected override string NewCollectionTypeName => Settings.EntityNewCollectionTypeName;

    protected override IBuilder<TypeBase> CreateResponseBuilder() => new TypeBaseBuilderWrapper(SourceModel);
}

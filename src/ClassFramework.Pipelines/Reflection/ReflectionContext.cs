namespace ClassFramework.Pipelines.Reflection;

public class ReflectionContext : ContextBase<Type>
{
    public ReflectionContext(Type sourceModel, PipelineSettings settings, IFormatProvider formatProvider)
        : base(sourceModel, settings, formatProvider)
    {
    }

    public override object CreateModel()
        => SourceModel.IsInterface
            ? new InterfaceBuilder()
            : new ClassBuilder();

    protected override string NewCollectionTypeName => Settings.EntityNewCollectionTypeName;
}

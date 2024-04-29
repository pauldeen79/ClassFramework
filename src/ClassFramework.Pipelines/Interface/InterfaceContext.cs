namespace ClassFramework.Pipelines.Interface;

public class InterfaceContext : ContextBase<TypeBase, Domain.Types.Interface>
{
    public InterfaceContext(TypeBase sourceModel, PipelineSettings settings, IFormatProvider formatProvider)
        : base(sourceModel, settings, formatProvider)
    {
    }

    protected override string NewCollectionTypeName => Settings.EntityNewCollectionTypeName;

    protected override IBuilder<Domain.Types.Interface> CreateResponseBuilder() => new InterfaceBuilderWrapper();
}

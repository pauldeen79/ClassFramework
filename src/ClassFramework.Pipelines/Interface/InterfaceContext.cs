namespace ClassFramework.Pipelines.Interface;

public class InterfaceContext(TypeBase sourceModel, PipelineSettings settings, IFormatProvider formatProvider, CancellationToken cancellationToken) : ContextBase<TypeBase>(sourceModel, settings, formatProvider, cancellationToken)
{
    protected override string NewCollectionTypeName => Settings.EntityNewCollectionTypeName;

    public InterfaceBuilder Builder => _wrappedBuilder.Builder;

    private readonly InterfaceBuilderWrapper _wrappedBuilder = new();
}

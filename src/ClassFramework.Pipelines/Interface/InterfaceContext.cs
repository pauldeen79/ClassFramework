namespace ClassFramework.Pipelines.Interface;

public class InterfaceContext(TypeBase sourceModel, PipelineSettings settings, IFormatProvider formatProvider) : ContextBase<TypeBase>(sourceModel, settings, formatProvider)
{
    protected override string NewCollectionTypeName => Settings.EntityNewCollectionTypeName;

    public InterfaceBuilder Builder => _wrappedBuilder.Builder;

    private readonly InterfaceBuilderWrapper _wrappedBuilder = new();
}

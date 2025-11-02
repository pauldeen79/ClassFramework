namespace ClassFramework.Pipelines.Interface;

public class InterfaceContext(TypeBase sourceModel, PipelineSettings settings, IFormatProvider formatProvider, CancellationToken cancellationToken) : ContextBase<TypeBase>(sourceModel, settings, formatProvider, cancellationToken)
{
    protected override string NewCollectionTypeName => Settings.EntityNewCollectionTypeName;

    public IEnumerable<Property> GetSourceProperties()
        => SourceModel.Properties.Where(x => SourceModel.IsMemberValidForBuilderClass(x, Settings));

    public InterfaceBuilder Builder { get; } = new();

    public override object GetResponseBuilder() => Builder;

    public override object GetResponseEntity() => Builder.Build();

    public override bool SourceModelHasNoProperties() => SourceModel.Properties.Count == 0;
}

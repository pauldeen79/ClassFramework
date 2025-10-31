namespace ClassFramework.Pipelines;

//TODO: Refactor this class so we don't need to inherit from ContextBase
public class PropertyContext(Property model, PipelineSettings settings, IFormatProvider formatProvider, string typeName, string newCollectionTypeName, CancellationToken cancellationToken) : ContextBase<Property>(model, settings, formatProvider, cancellationToken)
{
    public string TypeName { get; } = typeName.IsNotNull(nameof(typeName));

    protected override string NewCollectionTypeName { get; } = newCollectionTypeName.IsNotNull(nameof(newCollectionTypeName));

    public override object GetResponse()
    {
        throw new NotImplementedException();
    }

    public override bool HasNoProperties()
    {
        throw new NotImplementedException();
    }
}

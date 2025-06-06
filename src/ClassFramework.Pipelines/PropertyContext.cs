namespace ClassFramework.Pipelines;

public class PropertyContext(Property model, PipelineSettings settings, IFormatProvider formatProvider, string typeName, string newCollectionTypeName, CancellationToken cancellationToken) : ContextBase<Property>(model, settings, formatProvider, cancellationToken)
{
    public string TypeName { get; } = typeName.IsNotNull(nameof(typeName));

    protected override string NewCollectionTypeName { get; } = newCollectionTypeName.IsNotNull(nameof(newCollectionTypeName));
}

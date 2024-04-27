namespace ClassFramework.Pipelines;

public class PropertyContext : ContextBase<Property>
{
    public PropertyContext(Property model, PipelineSettings settings, IFormatProvider formatProvider, string typeName, string newCollectionTypeName)
        : base(model, settings, formatProvider)
    {
        TypeName = typeName.IsNotNull(nameof(typeName));
        NewCollectionTypeName = newCollectionTypeName.IsNotNull(nameof(newCollectionTypeName));
    }

    public string TypeName { get; }

    public override object CreateModel() => new PropertyBuilder();

    protected override string NewCollectionTypeName { get; }
}

namespace ClassFramework.Pipelines.Builders;

public partial class MetadataBuilder
{
    public MetadataBuilder(string name, object? value)
    {
        ArgumentGuard.IsNotNullOrEmpty(name, nameof(name));

        _name = name;
        _value = value;
    }
}

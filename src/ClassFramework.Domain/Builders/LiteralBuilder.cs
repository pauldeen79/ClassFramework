namespace ClassFramework.Domain.Builders;

public partial class LiteralBuilder
{
    public LiteralBuilder(string value, object? originalValue = null)
    {
        ArgumentGuard.IsNotNullOrEmpty(value, nameof(value));

        _value = value;
        _originalValue = originalValue;
    }
}

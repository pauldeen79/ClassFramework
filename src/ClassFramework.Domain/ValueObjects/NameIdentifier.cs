namespace ClassFramework.Domain.ValueObjects;

public partial record NameIdentifier
{
    public static implicit operator string(NameIdentifier source) => source.IsNotNull(nameof(source)).Value;
    public static implicit operator NameIdentifier(string source) => FromString(source);
    public static NameIdentifier FromString(string source) => new NameIdentifier(source);
}

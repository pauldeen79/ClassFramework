namespace ClassFramework.Domain.Builders.ValueObjects;

public partial class NameIdentifierBuilder
{
    public static implicit operator NameIdentifierBuilder(string source) => FromString(source);
    public static NameIdentifierBuilder FromString(string source) => new NameIdentifierBuilder().WithValue(source);
    public static implicit operator string(NameIdentifierBuilder source) => source.IsNotNull(nameof(source)).ToString();
    public override string ToString() => Value;
}

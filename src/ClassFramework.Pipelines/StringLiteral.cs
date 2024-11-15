namespace ClassFramework.Pipelines;

internal sealed class StringLiteral(string value) : IStringLiteral
{
    public string Value { get; } = value.IsNotNull(nameof(value));
}

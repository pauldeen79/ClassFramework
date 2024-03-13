﻿namespace ClassFramework.Pipelines.Extensions;

internal sealed class StringLiteral : IStringLiteral
{
    public StringLiteral(string value)
    {
        Value = value.IsNotNull(nameof(value));
    }

    public string Value { get; }
}

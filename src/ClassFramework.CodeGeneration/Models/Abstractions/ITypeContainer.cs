﻿namespace ClassFramework.CodeGeneration.Models.Abstractions;

internal interface ITypeContainer
{
    [Required] string TypeName { get; }
    bool IsNullable { get; }
    bool IsValueType { get; }
    [Required][ValidateObject] IReadOnlyCollection<ITypeContainer> GenericTypeArguments { get; }
}

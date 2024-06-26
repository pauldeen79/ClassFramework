﻿namespace ClassFramework.CodeGeneration.Models.Pipelines;

internal interface ITypenameMapping
{
    [Required] string SourceTypeName { get; }
    [Required] string TargetTypeName { get; }
    [Required][ValidateObject] IReadOnlyCollection<IMetadata> Metadata { get; }
}

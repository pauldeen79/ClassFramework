﻿namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableInheritFromInterfacesCoreEntities : ImmutableInheritFromInterfacesCSharpClassBase
{
    public ImmutableInheritFromInterfacesCoreEntities(IPipelineService pipelineService, ICsharpExpressionDumper csharpExpressionDumper) : base(pipelineService, csharpExpressionDumper)
    {
    }

    public override async Task<IEnumerable<TypeBase>> GetModel() => await GetEntities((await GetCoreModels().ConfigureAwait(false)).Select(x => x.ToBuilder().AddInterfaces($"Test.Domain.Abstractions.{x.Name}").Build()).ToArray(), "Test.Domain").ConfigureAwait(false);

    public override string Path => "Test.Domain";
}

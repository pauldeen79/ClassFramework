﻿namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableCoreBuilderExtensions(IPipelineService pipelineService) : ImmutableCSharpClassBase(pipelineService)
{
    public override async Task<IEnumerable<TypeBase>> GetModel() => await GetBuilderExtensions(await GetCoreModels().ConfigureAwait(false), "Test.Domain.Builders", "Test.Domain", "Test.Domain.Extensions").ConfigureAwait(false);

    public override string Path => "Test.Domain/Extensions";
}

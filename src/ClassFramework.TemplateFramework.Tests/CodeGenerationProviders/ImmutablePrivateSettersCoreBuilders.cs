﻿namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutablePrivateSettersCoreBuilders(IPipelineService pipelineService) : ImmutablePrivateSettersCSharpClassBase(pipelineService)
{
    public override async Task<IEnumerable<TypeBase>> GetModel() => await GetBuilders(await GetCoreModels().ConfigureAwait(false), "Test.Domain.Builders", "Test.Domain").ConfigureAwait(false);

    public override string Path => "Test.Domain/Builders";
}

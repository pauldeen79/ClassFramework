﻿namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ObservableCoreEntities(IPipelineService pipelineService) : ObservableCSharpClassBase(pipelineService)
{
    public override async Task<IEnumerable<TypeBase>> GetModel() => await GetEntities(await GetCoreModels().ConfigureAwait(false), "Test.Domain").ConfigureAwait(false);

    public override string Path => "Test.Domain";
}

﻿namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class TemplateFrameworkEntities(IPipelineService pipelineService) : ImmutableCSharpClassBase(pipelineService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken) => GetEntitiesAsync(GetTemplateFrameworkModels(), "ClassFramework.TemplateFramework");

    public override string Path => "ClassFramework.TemplateFramework";
}

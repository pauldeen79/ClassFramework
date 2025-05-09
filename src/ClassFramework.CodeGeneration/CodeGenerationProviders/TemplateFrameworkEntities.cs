﻿namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class TemplateFrameworkEntities(IPipelineService pipelineService) : ClassFrameworkCSharpClassBase(pipelineService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken) => GetEntities(GetTemplateFrameworkModels(), "ClassFramework.TemplateFramework");

    public override string Path => "ClassFramework.TemplateFramework";
}

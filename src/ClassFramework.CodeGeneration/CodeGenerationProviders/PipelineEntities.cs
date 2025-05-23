﻿namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class PipelineEntities(IPipelineService pipelineService) : ClassFrameworkCSharpClassBase(pipelineService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken) => GetEntities(GetPipelineModels(), "ClassFramework.Pipelines");

    public override string Path => "ClassFramework.Pipelines";
}

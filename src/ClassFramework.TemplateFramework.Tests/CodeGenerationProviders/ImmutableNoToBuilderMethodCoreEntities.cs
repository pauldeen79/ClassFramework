﻿namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableNoToBuilderMethodCoreEntities(IPipelineService pipelineService) : ImmutableNoToBuilderMethodCSharpClassBase(pipelineService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken) => GetEntities(GetCoreModels(), "Test.Domain");

    public override string Path => "Test.Domain";
}

﻿namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class CrossCuttingAbstractionsInterfaces(IPipelineService pipelineService) : CrossCuttingClassBase(pipelineService)
{
    public override string Path => "CrossCutting.Utilities.Parsers/Abstractions";

    protected override bool EnableEntityInheritance => true;

    public override Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken)
        => GetEntityInterfaces(GetCrossCuttingAbstractionsInterfaces(), CurrentNamespace.GetParentNamespace(), CurrentNamespace);
}

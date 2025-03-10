﻿namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class MultipleInterfacesAbstractionsBuildersInterfaces(IPipelineService pipelineService) : MultipleInterfacesBase(pipelineService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken) => GetBuilderInterfaces(GetAbstractionsTypes(), CurrentNamespace, "ClassFramework.Domain.Abstractions", CurrentNamespace);

    public override string Path => "ClassFramework.Domain/Builders/Abstractions";

    protected override bool EnableEntityInheritance => true;
}

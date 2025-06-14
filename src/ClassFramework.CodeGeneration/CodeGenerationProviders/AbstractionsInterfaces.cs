﻿namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class AbstractionsInterfaces(IPipelineService pipelineService) : ClassFrameworkCSharpClassBase(pipelineService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken) => GetEntityInterfacesAsync(GetAbstractionsInterfacesAsync(), "ClassFramework.Domain", "ClassFramework.Domain.Abstractions");

    public override string Path => "ClassFramework.Domain/Abstractions";

    protected override bool EnableEntityInheritance => true;
}

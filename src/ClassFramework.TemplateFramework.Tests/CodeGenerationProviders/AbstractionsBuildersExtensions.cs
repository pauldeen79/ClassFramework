﻿namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class AbstractionsBuildersExtensions(IPipelineService pipelineService) : ImmutableCSharpClassBase(pipelineService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken) => GetBuilderExtensionsAsync(GetAbstractionsInterfacesAsync(), "Test.Domain.Builders.Abstractions", "Test.Domain.Abstractions", "Test.Domain.Builders.Extensions");

    public override string Path => "Test.Domain/Builders/Extensions";

    protected override bool EnableEntityInheritance => true;
}

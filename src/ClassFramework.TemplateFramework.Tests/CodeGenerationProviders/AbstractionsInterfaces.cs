﻿namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class AbstractionsInterfaces : ImmutableCSharpClassBase
{
    public AbstractionsInterfaces(IPipelineService pipelineService, ICsharpExpressionDumper csharpExpressionDumper) : base(pipelineService, csharpExpressionDumper)
    {
    }

    public override string Path => "Test.Domain/Abstractions";

    public override async Task<IEnumerable<TypeBase>> GetModel() => await GetEntityInterfaces(await GetAbstractionsInterfaces().ConfigureAwait(false), "Test.Domain", "Test.Domain.Abstractions").ConfigureAwait(false);

    protected override bool EnableEntityInheritance => true;
}

﻿namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class OverrideTypeEntities : ImmutableCSharpClassBase
{
    public OverrideTypeEntities(IPipelineService pipelineService, ICsharpExpressionDumper csharpExpressionDumper) : base(pipelineService, csharpExpressionDumper)
    {
    }

    public override string Path => "Test.Domain/Types";

    protected override bool EnableEntityInheritance => true;
    protected override async Task<TypeBase?> GetBaseClass() => await CreateBaseClass(typeof(IAbstractBase), "Test.Domain").ConfigureAwait(false);

    public override async Task<IEnumerable<TypeBase>> GetModel()
        => await GetEntities(await GetOverrideModels(typeof(IAbstractBase)).ConfigureAwait(false), "Test.Domain.Types").ConfigureAwait(false);
}

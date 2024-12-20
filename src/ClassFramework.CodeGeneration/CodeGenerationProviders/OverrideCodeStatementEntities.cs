﻿namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class OverrideCodeStatementEntities(IPipelineService pipelineService) : ClassFrameworkCSharpClassBase(pipelineService)
{
    public override string Path => "ClassFramework.Domain/CodeStatements";

    protected override bool EnableEntityInheritance => true;
    protected override async Task<Result<TypeBase>> GetBaseClass() => await CreateBaseClass(typeof(ICodeStatementBase), "ClassFramework.Domain").ConfigureAwait(false);

    public override async Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken)
        => await GetEntities(await GetOverrideModels(typeof(ICodeStatementBase)).ConfigureAwait(false), "ClassFramework.Domain.CodeStatements").ConfigureAwait(false);
}

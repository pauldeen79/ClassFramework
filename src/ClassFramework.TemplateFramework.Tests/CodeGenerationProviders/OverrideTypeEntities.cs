namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class OverrideTypeEntities(IPipelineService pipelineService) : ImmutableCSharpClassBase(pipelineService)
{
    public override string Path => "Test.Domain/Types";

    protected override bool EnableEntityInheritance => true;
    protected override async Task<Result<TypeBase>> GetBaseClass() => await CreateBaseClass(typeof(IAbstractBase), "Test.Domain").ConfigureAwait(false);

    public override async Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken)
        => await GetEntities(await GetOverrideModels(typeof(IAbstractBase)).ConfigureAwait(false), "Test.Domain.Types").ConfigureAwait(false);
}

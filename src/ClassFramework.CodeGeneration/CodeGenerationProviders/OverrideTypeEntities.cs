namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class OverrideTypeEntities : ClassFrameworkCSharpClassBase
{
    public OverrideTypeEntities(IPipelineService pipelineService) : base(pipelineService)
    {
    }

    public override string Path => "ClassFramework.Domain/Types";

    protected override bool EnableEntityInheritance => true;
    protected override async Task<TypeBase?> GetBaseClass() => await CreateBaseClass(typeof(ITypeBase), "ClassFramework.Domain").ConfigureAwait(false);

    public override async Task<IEnumerable<TypeBase>> GetModel()
        => await GetEntities(await GetOverrideModels(typeof(ITypeBase)).ConfigureAwait(false), "ClassFramework.Domain.Types").ConfigureAwait(false);
}

namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class OverrideTypeBuilders : ClassFrameworkCSharpClassBase
{
    public OverrideTypeBuilders(IPipelineService pipelineService) : base(pipelineService)
    {
    }

    public override string Path => "ClassFramework.Domain/Builders/Types";

    protected override bool EnableEntityInheritance => true;
    protected override bool CreateAsObservable => true;
    protected override async Task<TypeBase?> GetBaseClass() => await CreateBaseClass(typeof(ITypeBase), "ClassFramework.Domain").ConfigureAwait(false);

    public override async Task<IEnumerable<TypeBase>> GetModel()
        => await GetBuilders(await GetOverrideModels(typeof(ITypeBase)).ConfigureAwait(false), "ClassFramework.Domain.Builders.Types", "ClassFramework.Domain.Types").ConfigureAwait(false);
}

namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class CoreEntities : ClassFrameworkCSharpClassBase
{
    public CoreEntities(IPipelineService pipelineService) : base(pipelineService)
    {
    }

    public override async Task<IEnumerable<TypeBase>> GetModel() => await GetEntities(await GetCoreModels().ConfigureAwait(false), "ClassFramework.Domain").ConfigureAwait(false);

    public override string Path => "ClassFramework.Domain";
}

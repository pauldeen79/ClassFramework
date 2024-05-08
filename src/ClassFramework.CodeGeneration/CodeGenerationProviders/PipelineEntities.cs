namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class PipelineEntities : ClassFrameworkCSharpClassBase
{
    public PipelineEntities(IPipelineService pipelineService) : base(pipelineService)
    {
    }

    public override async Task<IEnumerable<TypeBase>> GetModel() => await GetEntities(await GetPipelineModels().ConfigureAwait(false), "ClassFramework.Pipelines").ConfigureAwait(false);

    public override string Path => "ClassFramework.Pipelines";
}

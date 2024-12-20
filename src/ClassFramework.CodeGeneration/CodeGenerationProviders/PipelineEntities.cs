namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class PipelineEntities(IPipelineService pipelineService) : ClassFrameworkCSharpClassBase(pipelineService)
{
    public override async Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken) => await GetEntities(await GetPipelineModels().ConfigureAwait(false), "ClassFramework.Pipelines").ConfigureAwait(false);

    public override string Path => "ClassFramework.Pipelines";
}

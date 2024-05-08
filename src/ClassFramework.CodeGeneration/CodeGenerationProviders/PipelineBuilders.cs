namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class PipelineBuilders : ClassFrameworkCSharpClassBase
{
    public PipelineBuilders(IPipelineService pipelineService) : base(pipelineService)
    {
    }

    public override async Task<IEnumerable<TypeBase>> GetModel() => await GetBuilders(await GetPipelineModels().ConfigureAwait(false), "ClassFramework.Pipelines.Builders", "ClassFramework.Pipelines").ConfigureAwait(false);

    public override string Path => "ClassFramework.Pipelines/Builders";

    protected override bool CreateAsObservable => true;
}

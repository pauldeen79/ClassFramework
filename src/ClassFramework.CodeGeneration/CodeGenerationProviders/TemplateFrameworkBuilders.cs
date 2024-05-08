namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class TemplateFrameworkBuilders : ClassFrameworkCSharpClassBase
{
    public TemplateFrameworkBuilders(IPipelineService pipelineService) : base(pipelineService)
    {
    }

    public override async Task<IEnumerable<TypeBase>> GetModel() => await GetBuilders(await GetTemplateFrameworkModels().ConfigureAwait(false), "ClassFramework.TemplateFramework.Builders", "ClassFramework.TemplateFramework").ConfigureAwait(false);

    public override string Path => "ClassFramework.TemplateFramework/Builders";

    protected override bool CreateAsObservable => true;
}

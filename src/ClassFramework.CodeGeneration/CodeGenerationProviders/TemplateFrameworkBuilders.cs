namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class TemplateFrameworkBuilders(IPipelineService pipelineService) : ClassFrameworkCSharpClassBase(pipelineService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken) => GetBuildersAsync(GetTemplateFrameworkModelsAsync(), "ClassFramework.TemplateFramework.Builders", "ClassFramework.TemplateFramework");

    public override string Path => "ClassFramework.TemplateFramework/Builders";

    protected override bool CreateAsObservable => true;
}

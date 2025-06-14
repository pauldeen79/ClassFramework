namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class TemplateFrameworkBuilders(IPipelineService pipelineService) : ImmutableCSharpClassBase(pipelineService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken) => GetBuildersAsync(GetTemplateFrameworkModels(), "ClassFramework.TemplateFramework.Builders", "ClassFramework.TemplateFramework");

    public override string Path => "ClassFramework.TemplateFramework/Builders";
}

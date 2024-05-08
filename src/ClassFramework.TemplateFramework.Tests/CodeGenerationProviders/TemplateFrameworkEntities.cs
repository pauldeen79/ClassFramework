namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class TemplateFrameworkEntities : ImmutableCSharpClassBase
{
    public TemplateFrameworkEntities(IPipelineService pipelineService) : base(pipelineService)
    {
    }

    public override async Task<IEnumerable<TypeBase>> GetModel() => await GetEntities(await GetTemplateFrameworkModels().ConfigureAwait(false), "ClassFramework.TemplateFramework").ConfigureAwait(false);

    public override string Path => "ClassFramework.TemplateFramework";
}

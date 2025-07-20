namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class TestEntities(IPipelineService pipelineService) : ClassFrameworkCSharpClassBase(pipelineService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken) => GetEntitiesAsync(GetTestModelsAsync(), "ClassFramework.TemplateFramework.Tests");

    public override string Path => "ClassFramework.TemplateFramework.Tests";
    protected override bool UseBuilderLazyValues => true;
}

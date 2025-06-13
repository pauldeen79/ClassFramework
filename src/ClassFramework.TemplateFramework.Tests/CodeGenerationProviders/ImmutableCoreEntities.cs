namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableCoreEntities(IPipelineService pipelineService) : ImmutableCSharpClassBase(pipelineService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken) => GetEntities(GetCoreModels(), "Test.Domain");

    public override string Path => "Test.Domain";
}

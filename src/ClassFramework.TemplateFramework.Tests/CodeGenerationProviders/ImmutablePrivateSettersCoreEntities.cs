namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutablePrivateSettersCoreEntities(IPipelineService pipelineService) : ImmutablePrivateSettersCSharpClassBase(pipelineService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken) => GetEntities(GetCoreModels(), "Test.Domain");

    public override string Path => "Test.Domain";
}

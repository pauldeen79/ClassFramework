namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableNoToBuilderMethodCoreEntities(IPipelineService pipelineService) : ImmutableNoToBuilderMethodCSharpClassBase(pipelineService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken) => GetEntitiesAsync(GetCoreModelsAsync(), "Test.Domain");

    public override string Path => "Test.Domain";
}

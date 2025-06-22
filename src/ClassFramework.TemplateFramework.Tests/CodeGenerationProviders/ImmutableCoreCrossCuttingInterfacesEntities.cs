namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableCoreCrossCuttingInterfacesEntities(IPipelineService pipelineService) : ImmutableCSharpClassBase(pipelineService)
{
    protected override bool UseCrossCuttingInterfaces => true;

    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken) => GetEntitiesAsync(GetCoreModelsAsync(), "Test.Domain");

    public override string Path => "Test.Domain";
}

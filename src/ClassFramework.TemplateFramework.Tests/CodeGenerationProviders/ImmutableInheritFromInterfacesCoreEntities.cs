namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableInheritFromInterfacesCoreEntities(IPipelineService pipelineService) : ImmutableInheritFromInterfacesCSharpClassBase(pipelineService)
{
    public override async Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken) => await GetEntitiesAsync(GetInheritFromInterfacesModelsAsync(), "Test.Domain").ConfigureAwait(false);

    public override string Path => "Test.Domain";
}

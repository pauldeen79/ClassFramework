namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableNoToBuilderMethodCoreEntities(IPipelineService pipelineService) : ImmutableNoToBuilderMethodCSharpClassBase(pipelineService)
{
    public override async Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken) => await GetEntities(await GetCoreModels().ConfigureAwait(false), "Test.Domain").ConfigureAwait(false);

    public override string Path => "Test.Domain";
}

namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableInheritFromInterfacesAbstractionsInterfaces(IPipelineService pipelineService) : ImmutableInheritFromInterfacesCSharpClassBase(pipelineService)
{
    public override async Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken) => await GetEntityInterfaces(await GetCoreModels().ConfigureAwait(false), "Test.Domain", "Test.Abstractions").ConfigureAwait(false);

    public override string Path => "Test.Domain";
}

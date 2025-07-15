namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableInheritFromInterfacesCoreEntities(IPipelineService pipelineService) : ImmutableInheritFromInterfacesCSharpClassBase(pipelineService)
{
    public override async Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken) => await GetEntitiesAsync(Task.FromResult(Result.Success((await GetCoreModelsAsync().ConfigureAwait(false)).Value!.Select(x => x.ToBuilder().AddInterfaces($"Test.Abstractions.{x.Name}").Build()))), "Test.Domain").ConfigureAwait(false);

    public override string Path => "Test.Domain";
}

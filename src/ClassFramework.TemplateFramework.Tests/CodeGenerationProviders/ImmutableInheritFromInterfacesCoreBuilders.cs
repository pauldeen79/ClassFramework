namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableInheritFromInterfacesCoreBuilders(IPipelineService pipelineService) : ImmutableInheritFromInterfacesCSharpClassBase(pipelineService)
{
    public override async Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken) => await GetBuilders(Task.FromResult(Result.Success((await GetCoreModels().ConfigureAwait(false)).Value!.Select(x => x.ToBuilder().AddInterfaces($"Test.Domain.Abstractions.{x.Name}").Build()))), "Test.Domain.Builders", "Test.Domain").ConfigureAwait(false);

    public override string Path => "Test.Domain/Builders";
}

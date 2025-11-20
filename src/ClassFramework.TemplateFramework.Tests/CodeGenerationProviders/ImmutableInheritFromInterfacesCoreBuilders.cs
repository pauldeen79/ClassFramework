namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableInheritFromInterfacesCoreBuilders(ICommandService commandService) : ImmutableInheritFromInterfacesCSharpClassBase(commandService)
{
    public override async Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken token) => await GetBuildersAsync(GetInheritFromInterfacesModelsAsync(), "Test.Domain.Builders", "Test.Domain").ConfigureAwait(false);

    public override string Path => "Test.Domain/Builders";
}

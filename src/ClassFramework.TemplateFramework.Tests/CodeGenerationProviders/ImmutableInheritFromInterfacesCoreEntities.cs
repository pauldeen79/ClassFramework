namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableInheritFromInterfacesCoreEntities(ICommandService commandService) : ImmutableInheritFromInterfacesCSharpClassBase(commandService)
{
    public override async Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken token) => await GetEntitiesAsync(GetInheritFromInterfacesModelsAsync(), "Test.Domain").ConfigureAwait(false);

    public override string Path => "Test.Domain";
}

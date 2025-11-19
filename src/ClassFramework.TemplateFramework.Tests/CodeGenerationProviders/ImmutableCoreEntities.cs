namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableCoreEntities(ICommandService commandService) : ImmutableCSharpClassBase(commandService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken token) => GetEntitiesAsync(GetCoreModelsAsync(), "Test.Domain");

    public override string Path => "Test.Domain";
}

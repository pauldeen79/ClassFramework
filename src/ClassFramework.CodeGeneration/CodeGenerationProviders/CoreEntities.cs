namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class CoreEntities(ICommandService commandService) : ClassFrameworkCSharpClassBase(commandService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken token) => GetEntitiesAsync(GetCoreModelsAsync(), "ClassFramework.Domain");

    public override string Path => "ClassFramework.Domain";
}

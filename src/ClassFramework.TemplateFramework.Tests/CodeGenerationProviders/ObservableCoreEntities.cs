namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ObservableCoreEntities(ICommandService commandService) : ObservableCSharpClassBase(commandService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken token) => GetEntitiesAsync(GetCoreModelsAsync(), "Test.Domain");

    public override string Path => "Test.Domain";
}

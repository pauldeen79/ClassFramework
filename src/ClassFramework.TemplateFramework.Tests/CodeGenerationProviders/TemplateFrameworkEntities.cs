namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class TemplateFrameworkEntities(ICommandService commandService) : ImmutableCSharpClassBase(commandService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken token) => GetEntitiesAsync(GetTemplateFrameworkModelsAsync(), "ClassFramework.TemplateFramework");

    public override string Path => "ClassFramework.TemplateFramework";
}

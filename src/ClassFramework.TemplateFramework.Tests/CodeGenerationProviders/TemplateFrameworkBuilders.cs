namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class TemplateFrameworkBuilders(ICommandService commandService) : ImmutableCSharpClassBase(commandService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken token) => GetBuildersAsync(GetTemplateFrameworkModelsAsync(), "ClassFramework.TemplateFramework.Builders", "ClassFramework.TemplateFramework");

    public override string Path => "ClassFramework.TemplateFramework/Builders";
}

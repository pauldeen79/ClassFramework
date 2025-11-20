namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableCoreBuilderExtensions(ICommandService commandService) : ImmutableCSharpClassBase(commandService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken token) => GetBuilderExtensionsAsync(GetCoreModelsAsync(), "Test.Domain.Builders", "Test.Domain", "Test.Domain.Extensions");

    public override string Path => "Test.Domain/Extensions";
}

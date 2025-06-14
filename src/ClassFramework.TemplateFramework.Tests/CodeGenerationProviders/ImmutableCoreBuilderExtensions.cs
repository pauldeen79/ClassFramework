namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableCoreBuilderExtensions(IPipelineService pipelineService) : ImmutableCSharpClassBase(pipelineService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken) => GetBuilderExtensionsAsync(GetCoreModelsAsync(), "Test.Domain.Builders", "Test.Domain", "Test.Domain.Extensions");

    public override string Path => "Test.Domain/Extensions";
}

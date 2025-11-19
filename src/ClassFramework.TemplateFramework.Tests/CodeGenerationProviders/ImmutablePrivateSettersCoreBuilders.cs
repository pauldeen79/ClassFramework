namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutablePrivateSettersCoreBuilders(ICommandService commandService) : ImmutablePrivateSettersCSharpClassBase(commandService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken token) => GetBuildersAsync(GetCoreModelsAsync(), "Test.Domain.Builders", "Test.Domain");

    public override string Path => "Test.Domain/Builders";
}

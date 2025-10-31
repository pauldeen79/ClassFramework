namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableCoreCrossCuttingInterfacesBuilders(ICommandService commandService) : ImmutableCSharpClassBase(commandService)
{
    protected override bool UseCrossCuttingInterfaces => true;

    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken) => GetBuildersAsync(GetCoreModelsAsync(), "Test.Domain.Builders", "Test.Domain");

    public override string Path => "Test.Domain/Builders";
}

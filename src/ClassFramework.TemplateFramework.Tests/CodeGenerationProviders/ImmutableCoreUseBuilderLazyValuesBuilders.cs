namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableCoreUseBuilderLazyValuesBuilders(ICommandService commandService) : ImmutableCSharpClassBase(commandService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken token) => GetBuildersAsync(GetCoreModelsAsync(), "Test.Domain.Builders", "Test.Domain");

    public override string Path => "Test.Domain/Builders";
    protected override bool UseBuilderLazyValues => true;
}

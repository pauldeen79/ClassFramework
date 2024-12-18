namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableCoreErrorBuilders(IPipelineService pipelineService) : ImmutableCSharpClassBase(pipelineService)
{
    protected override string SetMethodNameFormatString => "With{$property.Kaboom}";

    public override async Task<IEnumerable<TypeBase>> GetModel() => await GetBuilders(await GetCoreModels().ConfigureAwait(false), "Test.Domain.Builders", "Test.Domain").ConfigureAwait(false);

    public override string Path => "Test.Domain/Builders";
}

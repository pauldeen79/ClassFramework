namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableCoreErrorBuilders(IPipelineService pipelineService) : ImmutableCSharpClassBase(pipelineService)
{
    protected override string SetMethodNameFormatString => "With{property.Kaboom}";

    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken) => GetBuilders(GetCoreModels(), "Test.Domain.Builders", "Test.Domain");

    public override string Path => "Test.Domain/Builders";
}

namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableCoreModelErrorBuilders(IPipelineService pipelineService) : ImmutableCSharpClassBase(pipelineService)
{
    protected override string SetMethodNameFormatString => "With{property.Kaboom}";

    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken) => GetBuilders(Task.FromResult(Result.Error<IEnumerable<TypeBase>>("Kaboom")), "Test.Domain.Builders", "Test.Domain");

    public override string Path => "Test.Domain/Builders";
}

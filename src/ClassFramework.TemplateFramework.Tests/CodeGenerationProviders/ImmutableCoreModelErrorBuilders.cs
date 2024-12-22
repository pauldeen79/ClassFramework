namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableCoreModelErrorBuilders(IPipelineService pipelineService) : ImmutableCSharpClassBase(pipelineService)
{
    protected override string SetMethodNameFormatString => "With{$property.Kaboom}";

    public override async Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken) => await GetBuilders(Result.Error<IEnumerable<TypeBase>>("Kaboom"), "Test.Domain.Builders", "Test.Domain").ConfigureAwait(false);

    public override string Path => "Test.Domain/Builders";
}

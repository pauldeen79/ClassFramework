namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class AbstractionsInterfaces(IPipelineService pipelineService) : ImmutableCSharpClassBase(pipelineService)
{
    public override string Path => "Test.Domain/Abstractions";

    public override async Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken) => await GetEntityInterfaces(await GetAbstractionsInterfaces().ConfigureAwait(false), "Test.Domain", "Test.Domain.Abstractions").ConfigureAwait(false);

    protected override bool EnableEntityInheritance => true;
}

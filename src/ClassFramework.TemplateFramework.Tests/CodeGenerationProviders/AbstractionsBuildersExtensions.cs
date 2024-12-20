namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class AbstractionsBuildersExtensions(IPipelineService pipelineService) : ImmutableCSharpClassBase(pipelineService)
{
    public override async Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken) => await GetBuilderExtensions(await GetAbstractionsInterfaces().ConfigureAwait(false), "Test.Domain.Builders.Abstractions", "Test.Domain.Abstractions", "Test.Domain.Builders.Extensions").ConfigureAwait(false);

    public override string Path => "Test.Domain/Builders/Extensions";

    protected override bool EnableEntityInheritance => true;
}

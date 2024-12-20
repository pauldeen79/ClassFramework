namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class AbstractionsInterfaces(IPipelineService pipelineService) : ClassFrameworkCSharpClassBase(pipelineService)
{
    public override async Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken) => await GetEntityInterfaces(await GetAbstractionsInterfaces().ConfigureAwait(false), "ClassFramework.Domain", "ClassFramework.Domain.Abstractions").ConfigureAwait(false);

    public override string Path => "ClassFramework.Domain/Abstractions";

    protected override bool EnableEntityInheritance => true;
}

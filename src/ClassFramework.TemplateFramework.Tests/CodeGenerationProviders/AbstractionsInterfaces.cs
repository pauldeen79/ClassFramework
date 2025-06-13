namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class AbstractionsInterfaces(IPipelineService pipelineService) : ImmutableCSharpClassBase(pipelineService)
{
    public override string Path => "Test.Domain/Abstractions";

    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken) => GetEntityInterfaces(GetAbstractionsInterfaces(), "Test.Domain", "Test.Domain.Abstractions");

    protected override bool EnableEntityInheritance => true;
}

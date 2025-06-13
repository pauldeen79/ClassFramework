namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class AbstractionsBuildersInterfaces(IPipelineService pipelineService) : ImmutableCSharpClassBase(pipelineService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken) => GetBuilderInterfaces(GetAbstractionsInterfaces(), "Test.Domain.Builders.Abstractions", "Test.Domain.Abstractions", "Test.Domain.Builders.Abstractions");

    public override string Path => "Test.Domain/Builders/Abstractions";

    protected override bool EnableEntityInheritance => true;
}

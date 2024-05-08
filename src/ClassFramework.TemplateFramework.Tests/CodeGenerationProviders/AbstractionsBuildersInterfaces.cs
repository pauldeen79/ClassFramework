namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class AbstractionsBuildersInterfaces : ImmutableCSharpClassBase
{
    public AbstractionsBuildersInterfaces(IPipelineService pipelineService) : base(pipelineService)
    {
    }

    public override async Task<IEnumerable<TypeBase>> GetModel() => await GetBuilderInterfaces(await GetAbstractionsInterfaces().ConfigureAwait(false), "Test.Domain.Builders.Abstractions", "Test.Domain.Abstractions", "Test.Domain.Builders.Abstractions").ConfigureAwait(false);

    public override string Path => "Test.Domain/Builders/Abstractions";

    protected override bool EnableEntityInheritance => true;
}

namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class AbstractionsBuildersExtensions : ImmutableCSharpClassBase
{
    public AbstractionsBuildersExtensions(IPipelineService pipelineService, ICsharpExpressionDumper csharpExpressionDumper) : base(pipelineService, csharpExpressionDumper)
    {
    }

    public override async Task<IEnumerable<TypeBase>> GetModel() => await GetBuilderExtensions(await GetAbstractionsInterfaces().ConfigureAwait(false), "Test.Domain.Builders.Abstractions", "Test.Domain.Abstractions", "Test.Domain.Builders.Extensions").ConfigureAwait(false);

    public override string Path => "Test.Domain/Builders/Extensions";

    protected override bool EnableEntityInheritance => true;
}

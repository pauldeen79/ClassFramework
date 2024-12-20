namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class OverrideTypeBuilders(IPipelineService pipelineService) : ImmutableCSharpClassBase(pipelineService)
{
    public override string Path => "Test.Domain/Builders/Types";

    protected override bool EnableEntityInheritance => true;
    protected override bool CreateAsObservable => true;
    protected override async Task<TypeBase?> GetBaseClass() => await CreateBaseClass(typeof(IAbstractBase), "Test.Domain").ConfigureAwait(false);

    public override async Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken)
        => await GetBuilders(await GetOverrideModels(typeof(IAbstractBase)).ConfigureAwait(false), "Test.Domain.Builders.Types", "Test.Domain.Types").ConfigureAwait(false);
}

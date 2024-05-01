namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class OverrideTypeBuilders : ImmutableCSharpClassBase
{
    public OverrideTypeBuilders(IMediator mediator) : base(mediator)
    {
    }

    public override string Path => "Test.Domain/Builders/Types";

    protected override bool EnableEntityInheritance => true;
    protected override bool CreateAsObservable => true;
    protected override async Task<TypeBase?> GetBaseClass() => await CreateBaseClass(typeof(IAbstractBase), "Test.Domain").ConfigureAwait(false);

    public override async Task<IEnumerable<TypeBase>> GetModel()
        => await GetBuilders(await GetOverrideModels(typeof(IAbstractBase)).ConfigureAwait(false), "Test.Domain.Builders.Types", "Test.Domain.Types").ConfigureAwait(false);
}

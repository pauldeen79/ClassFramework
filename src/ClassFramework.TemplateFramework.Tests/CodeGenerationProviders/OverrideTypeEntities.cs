namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class OverrideTypeEntities : ImmutableCSharpClassBase
{
    public OverrideTypeEntities(ICsharpExpressionDumper csharpExpressionDumper, IMediator mediator) : base(csharpExpressionDumper, mediator)
    {
    }

    public override string Path => "Test.Domain/Types";

    protected override bool EnableEntityInheritance => true;
    protected override async Task<Class?> GetBaseClass() => await CreateBaseClass(typeof(IAbstractBase), "Test.Domain").ConfigureAwait(false);

    public override async Task<IEnumerable<TypeBase>> GetModel()
        => await GetEntities(await GetOverrideModels(typeof(IAbstractBase)).ConfigureAwait(false), "Test.Domain.Types").ConfigureAwait(false);
}

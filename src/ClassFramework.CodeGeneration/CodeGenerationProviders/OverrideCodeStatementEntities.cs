namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class OverrideCodeStatementEntities : ClassFrameworkCSharpClassBase
{
    public OverrideCodeStatementEntities(IMediator mediator, ICsharpExpressionDumper csharpExpressionDumper) : base(mediator, csharpExpressionDumper)
    {
    }

    public override string Path => "ClassFramework.Domain/CodeStatements";

    protected override bool EnableEntityInheritance => true;
    protected override async Task<TypeBase?> GetBaseClass() => await CreateBaseClass(typeof(ICodeStatementBase), "ClassFramework.Domain").ConfigureAwait(false);

    public override async Task<IEnumerable<TypeBase>> GetModel()
        => await GetEntities(await GetOverrideModels(typeof(ICodeStatementBase)).ConfigureAwait(false), "ClassFramework.Domain.CodeStatements").ConfigureAwait(false);
}

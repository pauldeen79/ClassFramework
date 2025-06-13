namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class OverrideCodeStatementEntities(IPipelineService pipelineService) : ClassFrameworkCSharpClassBase(pipelineService)
{
    public override string Path => "ClassFramework.Domain/CodeStatements";

    protected override bool EnableEntityInheritance => true;
    protected override Task<Result<TypeBase>> GetBaseClass() => CreateBaseClass(typeof(ICodeStatementBase), "ClassFramework.Domain");

    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken)
        => GetEntities(GetOverrideModels(typeof(ICodeStatementBase)), "ClassFramework.Domain.CodeStatements");
}

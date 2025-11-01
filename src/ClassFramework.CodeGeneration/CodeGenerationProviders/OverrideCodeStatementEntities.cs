namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class OverrideCodeStatementEntities(ICommandService commandService) : ClassFrameworkCSharpClassBase(commandService)
{
    public override string Path => "ClassFramework.Domain/CodeStatements";

    protected override bool EnableEntityInheritance => true;
    protected override Task<Result<TypeBase>> GetBaseClassAsync() => CreateBaseClassAsync(typeof(ICodeStatementBase), "ClassFramework.Domain");

    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken)
        => GetEntitiesAsync(GetOverrideModelsAsync(typeof(ICodeStatementBase)), "ClassFramework.Domain.CodeStatements");
}

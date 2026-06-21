namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class OverrideCodeStatementBuilders(ICommandService commandService) : ClassFrameworkCSharpClassBase(commandService)
{
    public override string Path => "ClassFramework.Domain/Builders/CodeStatements";

    protected override bool EnableEntityInheritance => true;
    protected override bool CreateAsObservable => true;
    protected override Task<Result<TypeBase>> GetBaseClassAsync() => CreateBaseClassAsync(typeof(ICodeStatementBase), "ClassFramework.Domain");

    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken token)
        => GetBuildersAsync(GetOverrideModelsAsync(typeof(ICodeStatementBase)), "ClassFramework.Domain.Builders.CodeStatements", "ClassFramework.Domain.CodeStatements");
}

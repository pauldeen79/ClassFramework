namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class OverrideCodeStatementBuilders(IPipelineService pipelineService) : ClassFrameworkCSharpClassBase(pipelineService)
{
    public override string Path => "ClassFramework.Domain/Builders/CodeStatements";

    protected override bool EnableEntityInheritance => true;
    protected override bool CreateAsObservable => true;
    protected override bool UseBuilderAbstractionsTypeConversion => false; //quirk
    protected override Task<Result<TypeBase>> GetBaseClass() => CreateBaseClass(typeof(ICodeStatementBase), "ClassFramework.Domain");

    public override Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken)
        => GetBuilders(GetOverrideModels(typeof(ICodeStatementBase)), "ClassFramework.Domain.Builders.CodeStatements", "ClassFramework.Domain.CodeStatements");
}

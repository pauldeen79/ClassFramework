namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class AbstractionsBuildersInterfaces(IPipelineService pipelineService) : ClassFrameworkCSharpClassBase(pipelineService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken) => GetBuilderInterfaces(GetAbstractionsInterfaces(), "ClassFramework.Domain.Builders.Abstractions", "ClassFramework.Domain.Abstractions", "ClassFramework.Domain.Builders.Abstractions");

    public override string Path => "ClassFramework.Domain/Builders/Abstractions";
    
    protected override bool EnableEntityInheritance => true;
}

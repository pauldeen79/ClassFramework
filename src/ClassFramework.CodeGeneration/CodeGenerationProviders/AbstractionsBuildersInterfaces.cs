namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class AbstractionsBuildersInterfaces(IPipelineService pipelineService) : ClassFrameworkCSharpClassBase(pipelineService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken) => GetBuilderInterfaces(GetAbstractionsInterfaces(), "ClassFramework.Domain.Builders.Abstractions", "ClassFramework.Domain.Abstractions", "ClassFramework.Domain.Builders.Abstractions");

    public override string Path => "ClassFramework.Domain/Builders/Abstractions";
    
    protected override bool EnableEntityInheritance => true;
    //protected override string BuildMethodName => string.Empty; // can't create Build method because interfaces are used on multiple builders (multiple inheritance)
    protected override string BuildTypedMethodName => string.Empty; // can't create Build method because interfaces are used on multiple builders (multiple inheritance)
}

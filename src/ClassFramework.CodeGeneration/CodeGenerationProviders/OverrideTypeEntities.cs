namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class OverrideTypeEntities(IPipelineService pipelineService) : ClassFrameworkCSharpClassBase(pipelineService)
{
    public override string Path => "ClassFramework.Domain/Types";

    protected override bool EnableEntityInheritance => true;
    protected override bool UseBuilderAbstractionsTypeConversion => false; //quirk
    protected override Task<Result<TypeBase>> GetBaseClass() => CreateBaseClass(typeof(ITypeBase), "ClassFramework.Domain");

    public override Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken)
        => GetEntities(GetOverrideModels(typeof(ITypeBase)), "ClassFramework.Domain.Types");
}

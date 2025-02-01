namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class CoreEntities(IPipelineService pipelineService) : ClassFrameworkCSharpClassBase(pipelineService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken) => GetEntities(GetCoreModels(), "ClassFramework.Domain");

    public override string Path => "ClassFramework.Domain";

    protected override bool UseBuilderAbstractionsTypeConversion => false; //quirk
}

namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class CoreBuilders(IPipelineService pipelineService) : ClassFrameworkCSharpClassBase(pipelineService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken) => GetBuilders(GetCoreModels(), "ClassFramework.Domain.Builders", "ClassFramework.Domain");

    public override string Path => "ClassFramework.Domain/Builders";

    protected override bool CreateAsObservable => true;
}

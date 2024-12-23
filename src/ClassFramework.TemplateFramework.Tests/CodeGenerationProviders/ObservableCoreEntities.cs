namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ObservableCoreEntities(IPipelineService pipelineService) : ObservableCSharpClassBase(pipelineService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken) => GetEntities(GetCoreModels(), "Test.Domain");

    public override string Path => "Test.Domain";
}

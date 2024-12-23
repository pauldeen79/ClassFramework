namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ObservableCoreBuilders(IPipelineService pipelineService) : ObservableCSharpClassBase(pipelineService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken) => GetBuilders(GetCoreModels(), "Test.Domain.Builders", "Test.Domain");

    public override string Path => "Test.Domain/Builders";
}

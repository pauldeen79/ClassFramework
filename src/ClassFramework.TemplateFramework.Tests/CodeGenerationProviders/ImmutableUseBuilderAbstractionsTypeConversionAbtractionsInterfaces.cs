namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableUseBuilderAbstractionsTypeConversionAbstractionsInterfaces(IPipelineService pipelineService) : ImmutableUseBuilderAbstractionsTypeConversionCSharpClassBase(pipelineService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken) => GetEntityInterfaces(GetCoreModels(), "Test.Domain", "Test.Abstractions");

    public override string Path => "Test.Domain";
}

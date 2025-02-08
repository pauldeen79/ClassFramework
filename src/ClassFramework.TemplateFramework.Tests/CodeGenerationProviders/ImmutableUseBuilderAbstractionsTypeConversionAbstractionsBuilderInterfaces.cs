namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableUseBuilderAbstractionsTypeConversionAbstractionsBuilderInterfaces(IPipelineService pipelineService) : ImmutableUseBuilderAbstractionsTypeConversionCSharpClassBase(pipelineService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken) => GetBuilderInterfaces(GetCoreModels(), "Test.Domain.Builders", "Test.Domain", "Test.Abstractions");

    public override string Path => "Test.Domain";
}

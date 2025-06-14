namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableUseBuilderAbstractionsTypeConversionAbstractionsBuilderInterfaces(IPipelineService pipelineService) : ImmutableUseBuilderAbstractionsTypeConversionCSharpClassBase(pipelineService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken) => GetBuilderInterfacesAsync(GetCoreModelsAsync(), "Test.Domain.Builders", "Test.Domain", "Test.Abstractions");

    public override string Path => "Test.Domain";
}

namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableInheritFromInterfacesAbstractionsBuilderInterfaces(IPipelineService pipelineService) : ImmutableInheritFromInterfacesCSharpClassBase(pipelineService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken) => GetBuilderInterfaces(GetCoreModels(), "Test.Domain.Builders", "Test.Domain", "Test.Abstractions");

    public override string Path => "Test.Domain";
}

namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableInheritFromInterfacesAbstractionsInterfaces(IPipelineService pipelineService) : ImmutableInheritFromInterfacesCSharpClassBase(pipelineService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken) => GetEntityInterfacesAsync(GetCoreModelsAsync(), "Test.Domain", "Test.Abstractions");

    public override string Path => "Test.Domain";
}

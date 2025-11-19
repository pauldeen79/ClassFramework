namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableUseBuilderAbstractionsTypeConversionAbstractionsBuilderInterfaces(ICommandService commandService) : ImmutableUseBuilderAbstractionsTypeConversionCSharpClassBase(commandService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken token) => GetBuilderInterfacesAsync(GetCoreModelsAsync(), "Test.Domain.Builders", "Test.Domain", "Test.Abstractions");

    public override string Path => "Test.Domain";
}

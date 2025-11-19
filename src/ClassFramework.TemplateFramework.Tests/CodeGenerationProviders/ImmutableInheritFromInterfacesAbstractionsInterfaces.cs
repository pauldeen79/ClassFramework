namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableInheritFromInterfacesAbstractionsInterfaces(ICommandService commandService) : ImmutableInheritFromInterfacesCSharpClassBase(commandService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken token) => GetEntityInterfacesAsync(GetCoreModelsAsync(), "Test.Domain", "Test.Abstractions");

    public override string Path => "Test.Domain";
}

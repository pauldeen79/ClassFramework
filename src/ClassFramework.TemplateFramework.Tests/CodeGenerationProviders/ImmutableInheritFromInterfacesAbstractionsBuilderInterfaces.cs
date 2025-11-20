namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableInheritFromInterfacesAbstractionsBuilderInterfaces(ICommandService commandService) : ImmutableInheritFromInterfacesCSharpClassBase(commandService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken token) => GetBuilderInterfacesAsync(GetAbstractionsInterfacesAsync(), "Test.Abstractions.Builders", "Test.Abstractions", "Test.Abstractions.Builders");

    public override string Path => "Test.Domain";
}

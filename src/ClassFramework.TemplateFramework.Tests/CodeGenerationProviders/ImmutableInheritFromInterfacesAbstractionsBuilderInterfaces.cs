namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableInheritFromInterfacesAbstractionsBuilderInterfaces(ICommandService commandService) : ImmutableInheritFromInterfacesCSharpClassBase(commandService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken) => GetBuilderInterfacesAsync(GetAbstractionsInterfacesAsync(), "Test.Abstractions.Builders", "Test.Abstractions", "Test.Abstractions.Builders");

    public override string Path => "Test.Domain";
}

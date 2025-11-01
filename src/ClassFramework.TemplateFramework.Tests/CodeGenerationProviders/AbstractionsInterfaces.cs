namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class AbstractionsInterfaces(ICommandService commandService) : ImmutableCSharpClassBase(commandService)
{
    public override string Path => "Test.Domain/Abstractions";

    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken) => GetEntityInterfacesAsync(GetAbstractionsInterfacesAsync(), "Test.Domain", "Test.Domain.Abstractions");

    protected override bool EnableEntityInheritance => true;
}

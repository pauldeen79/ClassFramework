namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class AbstractionsBuildersInterfaces(ICommandService commandService) : ImmutableCSharpClassBase(commandService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken token) => GetBuilderInterfacesAsync(GetAbstractionsInterfacesAsync(), "Test.Domain.Builders.Abstractions", "Test.Domain.Abstractions", "Test.Domain.Builders.Abstractions");

    public override string Path => "Test.Domain/Builders/Abstractions";

    protected override bool EnableEntityInheritance => true;
}

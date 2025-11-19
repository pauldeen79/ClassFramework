namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class AbstractionsBuildersInterfaces(ICommandService commandService) : ClassFrameworkCSharpClassBase(commandService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken token) => GetBuilderInterfacesAsync(GetAbstractionsInterfacesAsync(), "ClassFramework.Domain.Builders.Abstractions", "ClassFramework.Domain.Abstractions", "ClassFramework.Domain.Builders.Abstractions");

    public override string Path => "ClassFramework.Domain/Builders/Abstractions";
    
    protected override bool EnableEntityInheritance => true;
}

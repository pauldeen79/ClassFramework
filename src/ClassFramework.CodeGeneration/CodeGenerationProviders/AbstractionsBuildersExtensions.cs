namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class AbstractionsBuildersExtensions(ICommandService commandService) : ClassFrameworkCSharpClassBase(commandService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken token) => GetBuilderExtensionsAsync(GetAbstractionsInterfacesAsync(), "ClassFramework.Domain.Builders.Abstractions", "ClassFramework.Domain.Abstractions", "ClassFramework.Domain.Builders.Extensions");

    public override string Path => "ClassFramework.Domain/Builders/Extensions";

    protected override bool EnableEntityInheritance => true;
}

namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class MultipleInterfacesAbstractionsBuildersExtensions(ICommandService commandService) : MultipleInterfacesBase(commandService)
{
    public override string Path => "ClassFramework.Domain/Builders/Extensions";

    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken token)
        => GetBuilderExtensionsAsync(GetAbstractionsTypesAsync(), "ClassFramework.Domain.Builders.Abstractions", "ClassFramework.Domain.Abstractions", CurrentNamespace);

    protected override bool EnableEntityInheritance => true;
}

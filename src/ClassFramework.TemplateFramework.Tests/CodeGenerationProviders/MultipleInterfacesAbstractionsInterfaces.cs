namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class MultipleInterfacesAbstractionsInterfaces(ICommandService commandService) : MultipleInterfacesBase(commandService)
{
    public override string Path => "ClassFramework.Domain/Abstractions";

    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken token)
        => GetEntityInterfacesAsync(GetAbstractionsTypesAsync(), CurrentNamespace.GetParentNamespace(), CurrentNamespace);

    protected override bool EnableEntityInheritance => true;
}

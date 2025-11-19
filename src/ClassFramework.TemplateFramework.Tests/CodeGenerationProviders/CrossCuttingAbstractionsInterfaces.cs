namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class CrossCuttingAbstractionsInterfaces(ICommandService commandService) : CrossCuttingClassBase(commandService)
{
    public override string Path => "CrossCutting.Utilities.Parsers/Abstractions";

    protected override bool EnableEntityInheritance => true;

    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken token)
        => GetEntityInterfacesAsync(GetCrossCuttingAbstractionsInterfacesAsync(), CurrentNamespace.GetParentNamespace(), CurrentNamespace);
}

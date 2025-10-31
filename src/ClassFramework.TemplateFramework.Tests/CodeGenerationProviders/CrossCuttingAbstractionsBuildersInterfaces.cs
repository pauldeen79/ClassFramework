namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class CrossCuttingAbstractionsBuildersInterfaces(ICommandService commandService) : CrossCuttingClassBase(commandService)
{
    public override string Path => "CrossCutting.Utilities.Parsers/Builders/Abstractions";

    protected override bool EnableEntityInheritance => true;

    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken)
        => GetBuilderInterfacesAsync(GetCrossCuttingAbstractionsInterfacesAsync(), CurrentNamespace, "CrossCutting.Utilities.Parsers.Abstractions", CurrentNamespace);
}

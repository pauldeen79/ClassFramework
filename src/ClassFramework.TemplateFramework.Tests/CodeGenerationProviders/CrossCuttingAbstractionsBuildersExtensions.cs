namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class CrossCuttingAbstractionsBuildersExtensions(ICommandService commandService) : CrossCuttingTestClassBase(commandService)
{
    public override string Path => "CrossCutting.Utilities.Parsers/Builders/Abstractions";

    protected override bool EnableEntityInheritance => true;

    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken token)
        => GetBuilderExtensionsAsync(GetCrossCuttingAbstractionsInterfacesAsync(), "CrossCutting.Utilities.Parsers.Builders.Abstractions", "CrossCutting.Utilities.Parsers.Abstractions", CurrentNamespace);
}

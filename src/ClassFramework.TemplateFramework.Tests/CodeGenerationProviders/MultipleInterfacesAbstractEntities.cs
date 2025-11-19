namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class MultipleInterfacesAbstractEntities(ICommandService commandService) : MultipleInterfacesBase(commandService)
{
    public override string Path => "ClassFramework.Domain";

    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken token)
        => GetEntitiesAsync(GetAbstractTypesAsync(), CurrentNamespace);

    protected override bool AddNullChecks => false; // not needed for abstract entities, because each derived class will do its own validation

    protected override bool EnableEntityInheritance => true;
    protected override bool IsAbstract => true;
}

namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class MultipleInterfacesAbstractEntities(IPipelineService pipelineService) : MultipleInterfacesBase(pipelineService)
{
    public override string Path => "ClassFramework.Domain";

    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken)
        => GetEntitiesAsync(GetAbstractTypesAsync(), CurrentNamespace);

    protected override bool AddNullChecks => false; // not needed for abstract entities, because each derived class will do its own validation

    protected override bool EnableEntityInheritance => true;
    protected override bool IsAbstract => true;
}

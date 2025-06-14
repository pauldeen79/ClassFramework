namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class AbstractEntities(IPipelineService pipelineService) : ImmutableCSharpClassBase(pipelineService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken) => GetEntitiesAsync(GetAbstractModelsAsync(), "Test.Domain");

    public override string Path => "Test.Domain";

    protected override bool AddNullChecks => false; // not needed for abstract entities, because each derived class will do its own validation

    protected override bool EnableEntityInheritance => true;
    protected override bool IsAbstract => true;
}

namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class MultipleInterfacesOverrideEntities(IPipelineService pipelineService) : MultipleInterfacesBase(pipelineService)
{
    public override string Path => "ClassFramework.Domain/Types";

    protected override bool EnableEntityInheritance => true;
    protected override bool CreateAsObservable => true;
    protected override Task<Result<TypeBase>> GetBaseClass() => CreateBaseClass(GetAbstractType(), "ClassFramework.Domain");

    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken)
        => GetEntities(GetOverrideTypes(), "ClassFramework.Domain.Types");
}

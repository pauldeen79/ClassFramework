namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class MultipleInterfacesOverrideBuilders(IPipelineService pipelineService) : MultipleInterfacesBase(pipelineService)
{
    public override string Path => "ClassFramework.Domain/Builders/Types";

    protected override bool EnableEntityInheritance => true;
    protected override bool CreateAsObservable => true;
    protected override Task<Result<TypeBase>> GetBaseClassAsync() => CreateBaseClassAsync(GetAbstractTypeAsync(), "ClassFramework.Domain");

    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken)
        => GetBuildersAsync(GetOverrideTypesAsync(), "ClassFramework.Domain.Builders.Types", "ClassFramework.Domain.Types");
}

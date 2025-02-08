namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class MultipleInterfacesAbstractionsInterfaces(IPipelineService pipelineService) : MultipleInterfacesBase(pipelineService)
{
    public override string Path => "ClassFramework.Domain/Abstractions";

    public override Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken)
        => GetEntityInterfaces(GetAbstractionsTypes(), CurrentNamespace.GetParentNamespace(), CurrentNamespace);

    protected override bool EnableEntityInheritance => true;
}

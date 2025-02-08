namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class MultipleInterfacesAbstractionsBuildersExtensions(IPipelineService pipelineService) : MultipleInterfacesBase(pipelineService)
{
    public override string Path => "ClassFramework.Domain/Builders/Extensions";

    public override Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken)
        => GetBuilderExtensions(GetAbstractionsTypes(), "ClassFramework.Domain.Builders.Abstractions", "ClassFramework.Domain.Abstractions", CurrentNamespace);

    protected override bool EnableEntityInheritance => true;
}

namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class MultipleInterfacesAbstractionsBuildersExtensions(IPipelineService pipelineService) : MultipleInterfacesBase(pipelineService)
{
    public override string Path => "ClassFramework.Domain/Builders/Extensions";

    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken)
        => GetBuilderExtensionsAsync(GetAbstractionsTypesAsync(), "ClassFramework.Domain.Builders.Abstractions", "ClassFramework.Domain.Abstractions", CurrentNamespace);

    protected override bool EnableEntityInheritance => true;
}

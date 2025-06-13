namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class CrossCuttingAbstractionsBuildersInterfaces(IPipelineService pipelineService) : CrossCuttingClassBase(pipelineService)
{
    public override string Path => "CrossCutting.Utilities.Parsers/Builders/Abstractions";

    protected override bool EnableEntityInheritance => true;

    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken)
        => GetBuilderInterfaces(GetCrossCuttingAbstractionsInterfaces(), CurrentNamespace, "CrossCutting.Utilities.Parsers.Abstractions", CurrentNamespace);
}

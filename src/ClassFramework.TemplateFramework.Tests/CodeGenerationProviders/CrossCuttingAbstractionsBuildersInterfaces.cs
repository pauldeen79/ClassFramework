namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class CrossCuttingAbstractionsBuildersInterfaces(IPipelineService pipelineService) : CrossCuttingClassBase(pipelineService)
{
    public override string Path => "CrossCutting.Utilities.Parsers/Builders/Abstractions";

    protected override bool EnableEntityInheritance => true;

    public override Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken)
        => GetBuilderInterfaces(GetCrossCuttingAbstractionsInterfaces(), "CrossCutting.Utilities.Parsers.Builders.Abstractions", "CrossCutting.Utilities.Parsers.Abstractions", "CrossCutting.Utilities.Parsers.Builders.Abstractions");
}

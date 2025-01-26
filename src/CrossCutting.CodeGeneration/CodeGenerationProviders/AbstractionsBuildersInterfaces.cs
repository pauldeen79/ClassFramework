namespace CrossCutting.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class AbstractionsBuildersInterfaces(IPipelineService pipelineService) : CrossCuttingCSharpClassBase(pipelineService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken) => GetBuilderInterfaces(GetAbstractionsInterfaces(), "CrossCutting.Utilities.Parsers.Builders.Abstractions", "CrossCutting.Utilities.Parsers.Abstractions", "CrossCutting.Utilities.Parsers.Builders.Abstractions");

    public override string Path => "CrossCutting.Utilities.Parsers/Builders/Abstractions";

    protected override bool EnableEntityInheritance => true;
}

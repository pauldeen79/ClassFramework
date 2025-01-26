namespace CrossCutting.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class AbstractionsInterfaces(IPipelineService pipelineService) : CrossCuttingCSharpClassBase(pipelineService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken) => GetEntityInterfaces(GetAbstractionsInterfaces(), "CrossCutting.Utilities.Parsers", "CrossCutting.Utilities.Parsers.Abstractions");

    public override string Path => "CrossCutting.Utilities.Parsers/Abstractions";

    protected override bool EnableEntityInheritance => true;
}

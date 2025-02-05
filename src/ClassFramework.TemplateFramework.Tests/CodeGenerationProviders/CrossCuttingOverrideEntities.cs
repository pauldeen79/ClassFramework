namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class CrossCuttingOverrideEntities(IPipelineService pipelineService) : CrossCuttingClassBase(pipelineService)
{
    public override string Path => "CrossCutting.Utilities.Parsers/FunctionCallArguments";

    protected override bool EnableEntityInheritance => true;
    protected override Task<Result<TypeBase>> GetBaseClass() => CreateCrossCuttingBaseClass("CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase", "CrossCutting.Utilities.Parsers");

    public override Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken)
        => GetEntities(GetCrossCuttingOverrideModels("CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase"), CurrentNamespace);
}

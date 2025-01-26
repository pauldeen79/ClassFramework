namespace CrossCutting.CodeGeneration.CodeGenerationProviders.FunctionCallArguments;

[ExcludeFromCodeCoverage]
public class OverrideEntities(IPipelineService pipelineService) : CrossCuttingCSharpClassBase(pipelineService)
{
    public override string Path => CrossCutting.CodeGeneration.Constants.Paths.FunctionCallArguments;

    protected override bool EnableEntityInheritance => true;
    protected override Task<Result<TypeBase>> GetBaseClass() => CreateBaseClass(typeof(IFunctionCallArgument), CrossCutting.CodeGeneration.Constants.Namespaces.UtilitiesParsers);

    public override Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken)
        => GetEntities(GetOverrideModels(typeof(IFunctionCallArgument)), CurrentNamespace);
}

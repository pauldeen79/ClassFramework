namespace CrossCutting.CodeGeneration.CodeGenerationProviders.FunctionCallArguments;

[ExcludeFromCodeCoverage]
public class OverrideBuilders(IPipelineService pipelineService) : CrossCuttingCSharpClassBase(pipelineService)
{
    public override string Path => Constants.Paths.FunctionCallArgumentBuilders;

    protected override bool EnableEntityInheritance => true;
    protected override bool CreateAsObservable => true;
    protected override Task<Result<TypeBase>> GetBaseClass() => CreateBaseClass(typeof(IFunctionCallArgument), Constants.Namespaces.UtilitiesParsers);
    protected override string BaseClassBuilderNamespace => Constants.Namespaces.UtilitiesParsersBuilders;

    public override Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken)
        => GetBuilders(
            GetOverrideModels(typeof(IFunctionCallArgument)),
            CurrentNamespace,
            Constants.Namespaces.UtilitiesParsersFunctionCallArguments);
}

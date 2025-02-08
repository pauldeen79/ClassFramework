namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class CrossCuttingOverrideBuilders(IPipelineService pipelineService) : CrossCuttingClassBase(pipelineService)
{
    public override string Path => "CrossCutting.Utilities.Parsers/Builders/FunctionCallArguments";

    protected override bool EnableEntityInheritance => true;
    protected override bool CreateAsObservable => true;
    protected override Task<Result<TypeBase>> GetBaseClass() => CreateCrossCuttingBaseClass("CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase", "CrossCutting.Utilities.Parsers");
    protected override string BaseClassBuilderNamespace => "CrossCutting.Utilities.Parsers.Builders";

    public override Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken)
        => GetBuilders(
            GetCrossCuttingOverrideModels("CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase"),
            CurrentNamespace,
            "CrossCutting.Utilities.Parsers.FunctionCallArguments");
}

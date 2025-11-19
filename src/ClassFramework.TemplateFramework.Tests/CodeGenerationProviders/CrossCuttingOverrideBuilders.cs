namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class CrossCuttingOverrideBuilders(ICommandService commandService) : CrossCuttingClassBase(commandService)
{
    public override string Path => "CrossCutting.Utilities.Parsers/Builders/FunctionCallArguments";

    protected override bool EnableEntityInheritance => true;
    protected override bool CreateAsObservable => true;
    protected override Task<Result<TypeBase>> GetBaseClassAsync() => CreateCrossCuttingBaseClassAsync("CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase", "CrossCutting.Utilities.Parsers");
    protected override string BaseClassBuilderNamespace => "CrossCutting.Utilities.Parsers.Builders";

    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken token)
        => GetBuildersAsync(
            GetCrossCuttingOverrideModelsAsync("CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase"),
            CurrentNamespace,
            "CrossCutting.Utilities.Parsers.FunctionCallArguments");
}

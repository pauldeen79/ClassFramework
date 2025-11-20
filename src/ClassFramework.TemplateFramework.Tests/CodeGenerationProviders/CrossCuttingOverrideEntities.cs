namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class CrossCuttingOverrideEntities(ICommandService commandService) : CrossCuttingClassBase(commandService)
{
    public override string Path => "CrossCutting.Utilities.Parsers/FunctionCallArguments";

    protected override bool EnableEntityInheritance => true;
    protected override Task<Result<TypeBase>> GetBaseClassAsync() => CreateCrossCuttingBaseClassAsync("CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase", "CrossCutting.Utilities.Parsers");

    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken token)
        => GetEntitiesAsync(GetCrossCuttingOverrideModelsAsync("CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase"), CurrentNamespace);
}

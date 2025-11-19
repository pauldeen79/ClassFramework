namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class PipelineEntities(ICommandService commandService) : ClassFrameworkCSharpClassBase(commandService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken token) => GetEntitiesAsync(GetPipelineModelsAsync(), "ClassFramework.Pipelines");

    public override string Path => "ClassFramework.Pipelines";
}

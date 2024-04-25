namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class PipelineEntities : ClassFrameworkCSharpClassBase
{
    public PipelineEntities(ICsharpExpressionDumper csharpExpressionDumper, IMediator mediator, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionDumper, mediator, builderExtensionPipeline, reflectionPipeline, interfacePipeline)
    {
    }

    public override async Task<IEnumerable<TypeBase>> GetModel() => await GetEntities(await GetPipelineModels().ConfigureAwait(false), "ClassFramework.Pipelines").ConfigureAwait(false);

    public override string Path => "ClassFramework.Pipelines";
}

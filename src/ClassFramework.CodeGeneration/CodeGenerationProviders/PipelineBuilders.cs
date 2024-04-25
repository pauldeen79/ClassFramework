namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class PipelineBuilders : ClassFrameworkCSharpClassBase
{
    public PipelineBuilders(ICsharpExpressionDumper csharpExpressionDumper, IMediator mediator, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionDumper, mediator, builderExtensionPipeline, reflectionPipeline, interfacePipeline)
    {
    }

    public override async Task<IEnumerable<TypeBase>> GetModel() => await GetBuilders(await GetPipelineModels().ConfigureAwait(false), "ClassFramework.Pipelines.Builders", "ClassFramework.Pipelines").ConfigureAwait(false);

    public override string Path => "ClassFramework.Pipelines/Builders";

    protected override bool CreateAsObservable => true;
}

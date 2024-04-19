namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class TemplateFrameworkBuilders : ClassFrameworkCSharpClassBase
{
    public TemplateFrameworkBuilders(ICsharpExpressionDumper csharpExpressionDumper, IPipeline<IConcreteTypeBuilder, BuilderContext> builderPipeline, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<IConcreteTypeBuilder, EntityContext> entityPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionDumper, builderPipeline, builderExtensionPipeline, entityPipeline, reflectionPipeline, interfacePipeline)
    {
    }

    public override async Task<IEnumerable<TypeBase>> GetModel() => await GetBuilders(await GetTemplateFrameworkModels().ConfigureAwait(false), "ClassFramework.TemplateFramework.Builders", "ClassFramework.TemplateFramework").ConfigureAwait(false);

    public override string Path => "ClassFramework.TemplateFramework/Builders";

    protected override bool CreateAsObservable => true;
}

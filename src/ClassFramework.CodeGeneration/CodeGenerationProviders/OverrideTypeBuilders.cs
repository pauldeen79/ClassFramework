namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class OverrideTypeBuilders : ClassFrameworkCSharpClassBase
{
    public OverrideTypeBuilders(ICsharpExpressionDumper csharpExpressionDumper, IPipeline<IConcreteTypeBuilder, BuilderContext> builderPipeline, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<IConcreteTypeBuilder, EntityContext> entityPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionDumper, builderPipeline, builderExtensionPipeline, entityPipeline, reflectionPipeline, interfacePipeline)
    {
    }

    public override string Path => "ClassFramework.Domain/Builders/Types";

    protected override bool EnableEntityInheritance => true;
    protected override bool CreateAsObservable => true;
    protected override async Task<Class?> GetBaseClass() => await CreateBaseClass(typeof(ITypeBase), "ClassFramework.Domain").ConfigureAwait(false);

    public override async Task<IEnumerable<TypeBase>> GetModel()
        => await GetBuilders(await GetOverrideModels(typeof(ITypeBase)).ConfigureAwait(false), "ClassFramework.Domain.Builders.Types", "ClassFramework.Domain.Types").ConfigureAwait(false);
}

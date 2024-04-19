namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class OverrideTypeEntities : ClassFrameworkCSharpClassBase
{
    public OverrideTypeEntities(ICsharpExpressionDumper csharpExpressionDumper, IPipeline<IConcreteTypeBuilder, BuilderContext> builderPipeline, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<IConcreteTypeBuilder, EntityContext> entityPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionDumper, builderPipeline, builderExtensionPipeline, entityPipeline, reflectionPipeline, interfacePipeline)
    {
    }

    public override string Path => "ClassFramework.Domain/Types";

    protected override bool EnableEntityInheritance => true;
    protected override async Task<Class?> GetBaseClass() => await CreateBaseClass(typeof(ITypeBase), "ClassFramework.Domain").ConfigureAwait(false);

    public override async Task<IEnumerable<TypeBase>> GetModel()
        => await GetEntities(await GetOverrideModels(typeof(ITypeBase)).ConfigureAwait(false), "ClassFramework.Domain.Types").ConfigureAwait(false);
}

namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class OverrideTypeBuilders : ImmutableCSharpClassBase
{
    public OverrideTypeBuilders(ICsharpExpressionDumper csharpExpressionDumper, IPipeline<IConcreteTypeBuilder, BuilderContext> builderPipeline, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<IConcreteTypeBuilder, EntityContext> entityPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionDumper, builderPipeline, builderExtensionPipeline, entityPipeline, reflectionPipeline, interfacePipeline)
    {
    }

    public override string Path => "Test.Domain/Builders/Types";

    protected override bool EnableEntityInheritance => true;
    protected override bool CreateAsObservable => true;
    protected override async Task<Class?> GetBaseClass() => await CreateBaseClass(typeof(IAbstractBase), "Test.Domain").ConfigureAwait(false);

    public override async Task<IEnumerable<TypeBase>> GetModel()
        => await GetBuilders(await GetOverrideModels(typeof(IAbstractBase)).ConfigureAwait(false), "Test.Domain.Builders.Types", "Test.Domain.Types").ConfigureAwait(false);
}

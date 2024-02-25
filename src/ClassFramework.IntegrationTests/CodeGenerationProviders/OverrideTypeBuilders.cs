namespace ClassFramework.IntegrationTests.CodeGenerationProviders;

public class OverrideTypeBuilders : TestCodeGenerationProviderBase
{
    public OverrideTypeBuilders(ICsharpExpressionCreator csharpExpressionCreator, IPipeline<IConcreteTypeBuilder, BuilderContext> builderPipeline, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<IConcreteTypeBuilder, EntityContext> entityPipeline, IPipeline<IConcreteTypeBuilder, OverrideEntityContext> overrideEntityPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionCreator, builderPipeline, builderExtensionPipeline, entityPipeline, overrideEntityPipeline, reflectionPipeline, interfacePipeline)
    {
    }

    public override string Path => "ClassFramework.Domain.POC/Builders/Types";

    protected override bool EnableEntityInheritance => true;
    protected override Class? BaseClass => CreateBaseclass(typeof(ITypeBase), "ClassFramework.Domain");

    public override IEnumerable<TypeBase> Model
        => GetBuilders(GetOverrideModels(typeof(ITypeBase)), "ClassFramework.Domain.Builders.Types", "ClassFramework.Domain.Types");
}

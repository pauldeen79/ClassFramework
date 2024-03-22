namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class CoreBuilders : ClassFrameworkCSharpClassBase
{
    public CoreBuilders(ICsharpExpressionDumper csharpExpressionDumper, IPipeline<IConcreteTypeBuilder, BuilderContext> builderPipeline, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<IConcreteTypeBuilder, EntityContext> entityPipeline, IPipeline<IConcreteTypeBuilder, OverrideEntityContext> overrideEntityPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionDumper, builderPipeline, builderExtensionPipeline, entityPipeline, overrideEntityPipeline, reflectionPipeline, interfacePipeline)
    {
    }

    public override IEnumerable<TypeBase> Model => GetBuilders(GetCoreModels(), "ClassFramework.Domain.Builders", "ClassFramework.Domain");

    public override string Path => "ClassFramework.Domain/Builders";

    protected override bool CreateAsObservable => true;
    protected override bool AddBackingFields => true;
}

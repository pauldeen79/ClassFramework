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
    protected override Class? BaseClass => CreateBaseclass(typeof(ITypeBase), "ClassFramework.Domain");

    public override IEnumerable<TypeBase> Model
        => GetBuilders(GetOverrideModels(typeof(ITypeBase)), "ClassFramework.Domain.Builders.Types", "ClassFramework.Domain.Types");
}

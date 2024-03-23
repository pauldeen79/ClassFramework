namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class AbstractBuilders : ClassFrameworkCSharpClassBase
{
    public AbstractBuilders(ICsharpExpressionDumper csharpExpressionDumper, IPipeline<IConcreteTypeBuilder, BuilderContext> builderPipeline, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<IConcreteTypeBuilder, EntityContext> entityPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionDumper, builderPipeline, builderExtensionPipeline, entityPipeline, reflectionPipeline, interfacePipeline)
    {
    }

    public override IEnumerable<TypeBase> Model => GetBuilders(GetAbstractModels(), "ClassFramework.Domain.Builders", "ClassFramework.Domain");

    public override string Path => "ClassFramework.Domain/Builders";

    protected override bool AddNullChecks => false; // not needed for abstract builders, because each derived class will do its own validation
    protected override bool AddBackingFields => true; // backing fields are added when using null checks... so we need to add this explicitly

    protected override bool EnableEntityInheritance => true;
    protected override bool EnableBuilderInhericance => true;
    protected override bool IsAbstract => true;

    // Do not generate 'With' methods. Do this on the interfaces instead.
    protected override string SetMethodNameFormatString => string.Empty;
    protected override string AddMethodNameFormatString => string.Empty;
}

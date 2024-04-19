namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class AbstractNonGenericBuilders : ClassFrameworkCSharpClassBase
{
    public AbstractNonGenericBuilders(ICsharpExpressionDumper csharpExpressionDumper, IPipeline<IConcreteTypeBuilder, BuilderContext> builderPipeline, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<IConcreteTypeBuilder, EntityContext> entityPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionDumper, builderPipeline, builderExtensionPipeline, entityPipeline, reflectionPipeline, interfacePipeline)
    {
    }

    public override async Task<IEnumerable<TypeBase>> GetModel() => await GetNonGenericBuilders(await GetAbstractModels().ConfigureAwait(false), "ClassFramework.Domain.Builders", "ClassFramework.Domain").ConfigureAwait(false);

    public override string Path => "ClassFramework.Domain/Builders";

    protected override bool AddNullChecks => false; // not needed for abstract builders, because each derived class will do its own validation
    protected override bool AddBackingFields => true; // backing fields are added when using null checks... so we need to add this explicitly

    protected override bool EnableEntityInheritance => true;
    protected override bool EnableBuilderInhericance => true;
    protected override bool IsAbstract => true;
    protected override bool CreateAsObservable => true;

    protected override string FilenameSuffix => ".nongeneric.template.generated";

    // Do not generate 'With' methods. Do this on the interfaces instead.
    protected override string SetMethodNameFormatString => string.Empty;
    protected override string AddMethodNameFormatString => string.Empty;
}

namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class AbstractionsInterfaces : ImmutableCSharpClassBase
{
    public AbstractionsInterfaces(ICsharpExpressionDumper csharpExpressionDumper, IPipeline<IConcreteTypeBuilder, BuilderContext> builderPipeline, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<IConcreteTypeBuilder, EntityContext> entityPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionDumper, builderPipeline, builderExtensionPipeline, entityPipeline, reflectionPipeline, interfacePipeline)
    {
    }

    public override string Path => "Test.Domain/Abstractions";

    public override async Task<IEnumerable<TypeBase>> GetModel() => await GetEntityInterfaces(await GetAbstractionsInterfaces().ConfigureAwait(false), "Test.Domain", "Test.Domain.Abstractions").ConfigureAwait(false);

    protected override bool EnableEntityInheritance => true;
}

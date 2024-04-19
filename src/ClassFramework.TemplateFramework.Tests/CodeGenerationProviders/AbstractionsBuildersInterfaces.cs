namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class AbstractionsBuildersInterfaces : ImmutableCSharpClassBase
{
    public AbstractionsBuildersInterfaces(ICsharpExpressionDumper csharpExpressionDumper, IPipeline<IConcreteTypeBuilder, BuilderContext> builderPipeline, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<IConcreteTypeBuilder, EntityContext> entityPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionDumper, builderPipeline, builderExtensionPipeline, entityPipeline, reflectionPipeline, interfacePipeline)
    {
    }

    public override async Task<IEnumerable<TypeBase>> GetModel() => await GetBuilderInterfaces(await GetAbstractionsInterfaces().ConfigureAwait(false), "Test.Domain.Builders.Abstractions", "Test.Domain.Abstractions", "Test.Domain.Builders.Abstractions").ConfigureAwait(false);

    public override string Path => "Test.Domain/Builders/Abstractions";

    protected override bool EnableEntityInheritance => true;
}

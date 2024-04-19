namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableInheritFromInterfacesAbstractionsBuilderInterfaces : ImmutableInheritFromInterfacesCSharpClassBase
{
    public ImmutableInheritFromInterfacesAbstractionsBuilderInterfaces(ICsharpExpressionDumper csharpExpressionDumper, IPipeline<IConcreteTypeBuilder, BuilderContext> builderPipeline, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<IConcreteTypeBuilder, EntityContext> entityPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionDumper, builderPipeline, builderExtensionPipeline, entityPipeline, reflectionPipeline, interfacePipeline)
    {
    }

    public override async Task<IEnumerable<TypeBase>> GetModel() => await GetBuilderInterfaces(await GetCoreModels().ConfigureAwait(false), "Test.Domain.Builders", "Test.Domain", "Test.Abstractions").ConfigureAwait(false);

    public override string Path => "Test.Domain";
}

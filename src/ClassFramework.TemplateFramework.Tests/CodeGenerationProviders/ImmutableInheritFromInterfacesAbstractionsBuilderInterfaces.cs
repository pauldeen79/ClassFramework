namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableInheritFromInterfacesAbstractionsBuilderInterfaces : ImmutableInheritFromInterfacesCSharpClassBase
{
    public ImmutableInheritFromInterfacesAbstractionsBuilderInterfaces(ICsharpExpressionDumper csharpExpressionDumper, IMediator mediator, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionDumper, mediator, builderExtensionPipeline, reflectionPipeline, interfacePipeline)
    {
    }

    public override async Task<IEnumerable<TypeBase>> GetModel() => await GetBuilderInterfaces(await GetCoreModels().ConfigureAwait(false), "Test.Domain.Builders", "Test.Domain", "Test.Abstractions").ConfigureAwait(false);

    public override string Path => "Test.Domain";
}

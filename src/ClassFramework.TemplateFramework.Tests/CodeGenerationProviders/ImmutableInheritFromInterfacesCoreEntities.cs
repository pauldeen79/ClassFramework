namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableInheritFromInterfacesCoreEntities : ImmutableInheritFromInterfacesCSharpClassBase
{
    public ImmutableInheritFromInterfacesCoreEntities(ICsharpExpressionDumper csharpExpressionDumper, IMediator mediator, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionDumper, mediator, builderExtensionPipeline, reflectionPipeline, interfacePipeline)
    {
    }

    public override async Task<IEnumerable<TypeBase>> GetModel() => await GetEntities((await GetCoreModels().ConfigureAwait(false)).Select(x => x.ToBuilder().AddInterfaces($"Test.Domain.Abstractions.{x.Name}").Build()).ToArray(), "Test.Domain").ConfigureAwait(false);

    public override string Path => "Test.Domain";
}

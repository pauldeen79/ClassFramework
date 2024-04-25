namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableInheritFromInterfacesCoreBuilders : ImmutableInheritFromInterfacesCSharpClassBase
{
    public ImmutableInheritFromInterfacesCoreBuilders(ICsharpExpressionDumper csharpExpressionDumper, IMediator mediator, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionDumper, mediator, builderExtensionPipeline, reflectionPipeline, interfacePipeline)
    {
    }

    public override async Task<IEnumerable<TypeBase>> GetModel() => await GetBuilders((await GetCoreModels().ConfigureAwait(false)).Select(x => x.ToBuilder().AddInterfaces($"Test.Domain.Abstractions.{x.Name}").Build()).ToArray(), "Test.Domain.Builders", "Test.Domain").ConfigureAwait(false);

    public override string Path => "Test.Domain/Builders";
}

namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableCoreBuilderExtensions : ImmutableCSharpClassBase
{
    public ImmutableCoreBuilderExtensions(ICsharpExpressionDumper csharpExpressionDumper, IMediator mediator, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionDumper, mediator, builderExtensionPipeline, reflectionPipeline, interfacePipeline)
    {
    }

    public override async Task<IEnumerable<TypeBase>> GetModel() => await GetBuilderExtensions(await GetCoreModels().ConfigureAwait(false), "Test.Domain.Builders", "Test.Domain", "Test.Domain.Extensions").ConfigureAwait(false);

    public override string Path => "Test.Domain/Extensions";
}

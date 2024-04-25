namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class AbstractionsInterfaces : ImmutableCSharpClassBase
{
    public AbstractionsInterfaces(ICsharpExpressionDumper csharpExpressionDumper, IMediator mediator, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionDumper, mediator, builderExtensionPipeline, reflectionPipeline, interfacePipeline)
    {
    }

    public override string Path => "Test.Domain/Abstractions";

    public override async Task<IEnumerable<TypeBase>> GetModel() => await GetEntityInterfaces(await GetAbstractionsInterfaces().ConfigureAwait(false), "Test.Domain", "Test.Domain.Abstractions").ConfigureAwait(false);

    protected override bool EnableEntityInheritance => true;
}

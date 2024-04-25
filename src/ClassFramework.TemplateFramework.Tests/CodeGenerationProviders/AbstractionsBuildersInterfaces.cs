namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class AbstractionsBuildersInterfaces : ImmutableCSharpClassBase
{
    public AbstractionsBuildersInterfaces(ICsharpExpressionDumper csharpExpressionDumper, IMediator mediator, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionDumper, mediator, builderExtensionPipeline, reflectionPipeline, interfacePipeline)
    {
    }

    public override async Task<IEnumerable<TypeBase>> GetModel() => await GetBuilderInterfaces(await GetAbstractionsInterfaces().ConfigureAwait(false), "Test.Domain.Builders.Abstractions", "Test.Domain.Abstractions", "Test.Domain.Builders.Abstractions").ConfigureAwait(false);

    public override string Path => "Test.Domain/Builders/Abstractions";

    protected override bool EnableEntityInheritance => true;
}

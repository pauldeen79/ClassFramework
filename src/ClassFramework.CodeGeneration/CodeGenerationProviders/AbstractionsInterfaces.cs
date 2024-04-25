namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class AbstractionsInterfaces : ClassFrameworkCSharpClassBase
{
    public AbstractionsInterfaces(ICsharpExpressionDumper csharpExpressionDumper, IMediator mediator, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionDumper, mediator, builderExtensionPipeline, reflectionPipeline, interfacePipeline)
    {
    }

    public override async Task<IEnumerable<TypeBase>> GetModel() => await GetEntityInterfaces(await GetAbstractionsInterfaces().ConfigureAwait(false), "ClassFramework.Domain", "ClassFramework.Domain.Abstractions").ConfigureAwait(false);

    public override string Path => "ClassFramework.Domain/Abstractions";

    protected override bool EnableEntityInheritance => true;
}

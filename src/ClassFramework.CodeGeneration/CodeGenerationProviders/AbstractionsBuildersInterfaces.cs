namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class AbstractionsBuildersInterfaces : ClassFrameworkCSharpClassBase
{
    public AbstractionsBuildersInterfaces(ICsharpExpressionDumper csharpExpressionDumper, IMediator mediator, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionDumper, mediator, builderExtensionPipeline, reflectionPipeline, interfacePipeline)
    {
    }

    public override async Task<IEnumerable<TypeBase>> GetModel() => await GetBuilderInterfaces(await GetAbstractionsInterfaces().ConfigureAwait(false), "ClassFramework.Domain.Builders.Abstractions", "ClassFramework.Domain.Abstractions", "ClassFramework.Domain.Builders.Abstractions").ConfigureAwait(false);

    public override string Path => "ClassFramework.Domain/Builders/Abstractions";

    protected override bool EnableEntityInheritance => true;
}

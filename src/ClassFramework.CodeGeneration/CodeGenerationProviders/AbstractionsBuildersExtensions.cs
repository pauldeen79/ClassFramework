namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class AbstractionsBuildersExtensions : ClassFrameworkCSharpClassBase
{
    public AbstractionsBuildersExtensions(ICsharpExpressionDumper csharpExpressionDumper, IPipeline<IConcreteTypeBuilder, BuilderContext> builderPipeline, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<IConcreteTypeBuilder, EntityContext> entityPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionDumper, builderPipeline, builderExtensionPipeline, entityPipeline, reflectionPipeline, interfacePipeline)
    {
    }

    public override async Task<IEnumerable<TypeBase>> GetModel() => await GetBuilderExtensions(await GetAbstractionsInterfaces().ConfigureAwait(false), "ClassFramework.Domain.Builders.Abstractions", "ClassFramework.Domain.Abstractions", "ClassFramework.Domain.Builders.Extensions").ConfigureAwait(false);

    public override string Path => "ClassFramework.Domain/Builders/Extensions";

    protected override bool EnableEntityInheritance => true;
}

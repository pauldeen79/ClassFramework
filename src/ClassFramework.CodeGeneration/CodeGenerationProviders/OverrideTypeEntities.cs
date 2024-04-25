namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class OverrideTypeEntities : ClassFrameworkCSharpClassBase
{
    public OverrideTypeEntities(ICsharpExpressionDumper csharpExpressionDumper, IMediator mediator, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionDumper, mediator, builderExtensionPipeline, reflectionPipeline, interfacePipeline)
    {
    }

    public override string Path => "ClassFramework.Domain/Types";

    protected override bool EnableEntityInheritance => true;
    protected override async Task<Class?> GetBaseClass() => await CreateBaseClass(typeof(ITypeBase), "ClassFramework.Domain").ConfigureAwait(false);

    public override async Task<IEnumerable<TypeBase>> GetModel()
        => await GetEntities(await GetOverrideModels(typeof(ITypeBase)).ConfigureAwait(false), "ClassFramework.Domain.Types").ConfigureAwait(false);
}

namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class OverrideCodeStatementEntities : ClassFrameworkCSharpClassBase
{
    public OverrideCodeStatementEntities(ICsharpExpressionDumper csharpExpressionDumper, IMediator mediator, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionDumper, mediator, builderExtensionPipeline, reflectionPipeline, interfacePipeline)
    {
    }

    public override string Path => "ClassFramework.Domain/CodeStatements";

    protected override bool EnableEntityInheritance => true;
    protected override async Task<Class?> GetBaseClass() => await CreateBaseClass(typeof(ICodeStatementBase), "ClassFramework.Domain").ConfigureAwait(false);

    public override async Task<IEnumerable<TypeBase>> GetModel()
        => await GetEntities(await GetOverrideModels(typeof(ICodeStatementBase)).ConfigureAwait(false), "ClassFramework.Domain.CodeStatements").ConfigureAwait(false);
}

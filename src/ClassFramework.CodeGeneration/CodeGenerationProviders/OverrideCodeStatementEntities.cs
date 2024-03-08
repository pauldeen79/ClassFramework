namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class OverrideCodeStatementEntities : ClassFrameworkCSharpClassBase
{
    public OverrideCodeStatementEntities(ICsharpExpressionCreator csharpExpressionCreator, IPipeline<IConcreteTypeBuilder, BuilderContext> builderPipeline, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<IConcreteTypeBuilder, EntityContext> entityPipeline, IPipeline<IConcreteTypeBuilder, OverrideEntityContext> overrideEntityPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionCreator, builderPipeline, builderExtensionPipeline, entityPipeline, overrideEntityPipeline, reflectionPipeline, interfacePipeline)
    {
    }

    public override string Path => "ClassFramework.Domain/CodeStatements";

    protected override bool EnableEntityInheritance => true;
    protected override Class? BaseClass => CreateBaseclass(typeof(ICodeStatementBase), "ClassFramework.Domain");

    public override IEnumerable<TypeBase> Model
        => GetEntities(GetOverrideModels(typeof(ICodeStatementBase)), "ClassFramework.Domain.CodeStatements");
}

namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

public class OverrideCodeStatementBuilders : ClassFrameworkCSharpClassBase
{
    public OverrideCodeStatementBuilders(ICsharpExpressionCreator csharpExpressionCreator, IPipeline<IConcreteTypeBuilder, BuilderContext> builderPipeline, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<IConcreteTypeBuilder, EntityContext> entityPipeline, IPipeline<IConcreteTypeBuilder, OverrideEntityContext> overrideEntityPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionCreator, builderPipeline, builderExtensionPipeline, entityPipeline, overrideEntityPipeline, reflectionPipeline, interfacePipeline)
    {
    }

    public override string Path => "ClassFramework.Domain/Builders/CodeStatements";

    protected override bool EnableEntityInheritance => true;
    protected override Class? BaseClass => CreateBaseclass(typeof(ICodeStatementBase), "ClassFramework.Domain");

    public override IEnumerable<TypeBase> Model
        => GetBuilders(GetOverrideModels(typeof(ICodeStatementBase)), "ClassFramework.Domain.Builders.CodeStatements", "ClassFramework.Domain.CodeStatements");
}

namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public abstract class ImmutableNoToBuilderMethodCSharpClassBase : ImmutableCSharpClassBase
{
    protected ImmutableNoToBuilderMethodCSharpClassBase(ICsharpExpressionDumper csharpExpressionDumper, IPipeline<IConcreteTypeBuilder, BuilderContext> builderPipeline, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<IConcreteTypeBuilder, EntityContext> entityPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionDumper, builderPipeline, builderExtensionPipeline, entityPipeline, reflectionPipeline, interfacePipeline)
    {
    }

    protected override SubVisibility SetterVisibility => SubVisibility.Private;
    protected override string ToBuilderFormatString => string.Empty;
    protected override string ToTypedBuilderFormatString => string.Empty;
}

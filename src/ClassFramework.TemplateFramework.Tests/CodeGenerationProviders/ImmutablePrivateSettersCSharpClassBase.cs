namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public abstract class ImmutablePrivateSettersCSharpClassBase : ImmutableCSharpClassBase
{
    protected ImmutablePrivateSettersCSharpClassBase(ICsharpExpressionDumper csharpExpressionDumper, IMediator mediator, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionDumper, mediator, builderExtensionPipeline, reflectionPipeline, interfacePipeline)
    {
    }

    protected override SubVisibility SetterVisibility => SubVisibility.Private;
    protected override bool AddSetters => true;
}

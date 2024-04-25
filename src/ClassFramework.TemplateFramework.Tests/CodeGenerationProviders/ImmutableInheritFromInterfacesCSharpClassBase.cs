namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public abstract class ImmutableInheritFromInterfacesCSharpClassBase : ImmutableCSharpClassBase
{
    protected ImmutableInheritFromInterfacesCSharpClassBase(ICsharpExpressionDumper csharpExpressionDumper, IMediator mediator, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionDumper, mediator, builderExtensionPipeline, reflectionPipeline, interfacePipeline)
    {
    }

    protected override bool InheritFromInterfaces => true;
}

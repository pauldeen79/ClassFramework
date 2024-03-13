namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public abstract class ObservableCSharpClassBase : ImmutableCSharpClassBase
{
    protected ObservableCSharpClassBase(ICsharpExpressionDumper csharpExpressionDumper, IPipeline<IConcreteTypeBuilder, BuilderContext> builderPipeline, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<IConcreteTypeBuilder, EntityContext> entityPipeline, IPipeline<IConcreteTypeBuilder, OverrideEntityContext> overrideEntityPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionDumper, builderPipeline, builderExtensionPipeline, entityPipeline, overrideEntityPipeline, reflectionPipeline, interfacePipeline)
    {
    }

    protected override Type EntityCollectionType => typeof(ObservableCollection<>);
    protected override Type EntityConcreteCollectionType => typeof(ObservableCollection<>);
    protected override Type BuilderCollectionType => typeof(ObservableCollection<>);
    protected override bool AddBackingFields => true;
    protected override bool CreateAsObservable => true;
    protected override bool AddFullConstructor => false;
    protected override bool AddPublicParameterlessConstructor => true;
}

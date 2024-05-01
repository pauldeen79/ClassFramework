namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public abstract class ObservableCSharpClassBase : ImmutableCSharpClassBase
{
    protected ObservableCSharpClassBase(IMediator mediator, ICsharpExpressionDumper csharpExpressionDumper) : base(mediator, csharpExpressionDumper)
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

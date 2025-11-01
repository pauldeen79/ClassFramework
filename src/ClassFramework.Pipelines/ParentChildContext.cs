namespace ClassFramework.Pipelines;

public class ParentChildContext<TParent, TChild>(TParent parentContext, TChild childContext, PipelineSettings settings)
{
    public TParent ParentContext { get; } = parentContext.IsNotNull(nameof(parentContext));
    public TChild ChildContext { get; } = childContext.IsNotNull(nameof(childContext));
    public PipelineSettings Settings { get; } = settings.IsNotNull(nameof(settings));
}

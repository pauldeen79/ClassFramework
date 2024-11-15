namespace ClassFramework.Pipelines;

public class ParentChildContext<TParentContext, TChild>(TParentContext parentContext, TChild childContext, PipelineSettings settings)
{
    public TParentContext ParentContext { get; } = parentContext.IsNotNull(nameof(parentContext));
    public TChild ChildContext { get; } = childContext.IsNotNull(nameof(childContext));
    public PipelineSettings Settings { get; } = settings.IsNotNull(nameof(settings));
}

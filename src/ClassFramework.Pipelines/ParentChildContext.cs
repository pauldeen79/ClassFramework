namespace ClassFramework.Pipelines;

public class ParentChildContext<TParentContext, TChild>
{
    public ParentChildContext(TParentContext parentContext, TChild childContext, PipelineSettings settings)
    {
        ParentContext = parentContext.IsNotNull(nameof(parentContext));
        ChildContext = childContext.IsNotNull(nameof(childContext));
        Settings = settings.IsNotNull(nameof(settings));
    }

    public TParentContext ParentContext { get; }
    public TChild ChildContext { get; }
    public PipelineSettings Settings { get; }
}

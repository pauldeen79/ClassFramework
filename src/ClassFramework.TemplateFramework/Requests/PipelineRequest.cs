namespace ClassFramework.TemplateFramework.Requests;

public class PipelineRequest<TModel, TContext> : IRequest<Result>
{
    public PipelineRequest(TModel model, TContext context)
    {
        Guard.IsNotNull(model);
        Guard.IsNotNull(context);

        Model = model;
        Context = context;
    }

    public TModel Model { get; }
    public TContext Context { get; }
}

namespace ClassFramework.TemplateFramework.Requests;

public class PipelineRequest<TContext, TResponse> : IRequest<Result<TResponse>>
{
    public PipelineRequest(TContext context)
    {
        Guard.IsNotNull(context);

        Context = context;
    }

    public TContext Context { get; }
}
